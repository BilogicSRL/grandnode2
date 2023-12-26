using Grand.Infrastructure.Configuration;
using Grand.SharedKernel.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

namespace Grand.Infrastructure.Themes
{
    /// <summary>
    /// Plugin manager
    /// </summary>
    public static class ThemeManager
    {
        #region Const

        private static object _synLock = new object();

        #endregion

        #region Fields

        private static DirectoryInfo _copyFolder;
        private static DirectoryInfo _themeFolder;
        private static ExtensionsConfig _config;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a collection of all referenced plugin assemblies that have been shadow copied
        /// </summary>
        public static IEnumerable<ThemeInfo> ReferencedThemes { get; set; }


        /// <summary>
        /// Load plugins
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Load(IMvcCoreBuilder mvcCoreBuilder, IConfiguration configuration)
        {
            _config = new ExtensionsConfig();
            configuration.GetSection("Extensions").Bind(_config);
            
            lock (_synLock)
            {
                if (mvcCoreBuilder == null)
                    throw new ArgumentNullException(nameof(mvcCoreBuilder));
                
                _themeFolder = new DirectoryInfo(CommonPath.ThemePath);
                _copyFolder = new DirectoryInfo(CommonPath.ThemesCopyPath);

                var referencedThemes = new List<ThemeInfo>();
                try
                {
                    Log.Information("Creating shadow copy theme folder and querying for dlls");
                    Directory.CreateDirectory(_themeFolder.FullName);
                    Directory.CreateDirectory(_copyFolder.FullName);
                    var binFiles = _copyFolder.GetFiles("*", SearchOption.AllDirectories);
                    if (_config.PluginShadowCopy)
                    {
                        //clear out shadow themes
                        foreach (var f in binFiles)
                        {
                            Log.Information("Deleting {FName}", f.Name);
                            try
                            {
                                File.Delete(f.FullName);
                            }
                            catch (Exception exc)
                            {
                                Log.Error(exc, "PluginManager");
                            }
                        }
                    }

                    //load description files
                    foreach (var theme in GetThemeInfo())
                    {
                        if (theme.SupportedVersion != GrandVersion.SupportedPluginVersion)
                        {
                            Log.Information("Incompatible theme {PluginSystemName}", theme.SystemName);
                            continue;
                        }

                        //some validation
                        if (string.IsNullOrWhiteSpace(theme.SystemName))
                            throw new Exception($"The theme '{theme.SystemName}' has no system name.");
                        if (referencedThemes.Contains(theme))
                            throw new Exception($"The theme with '{theme.SystemName}' system name is already defined");

                        try
                        {
                            if (!_config.PluginShadowCopy)
                            {
                                //remove deps.json files 
                                var depsFiles = theme.OriginalAssemblyFile.Directory!.GetFiles("*.deps.json", SearchOption.TopDirectoryOnly);
                                foreach (var f in depsFiles)
                                {
                                    try
                                    {
                                        File.Delete(f.FullName);
                                    }
                                    catch (Exception exc)
                                    {
                                        Log.Error(exc, "ThemeManager");
                                    }
                                }
                            }

                            //main theme file
                            AddApplicationPart(mvcCoreBuilder, theme.ReferencedAssembly, theme.SystemName, theme.ThemeFileName);
                            
                            referencedThemes.Add(theme);
                        }
                        catch (ReflectionTypeLoadException ex)
                        {
                            var msg = $"Theme '{theme.FriendlyName}'. ";
                            msg = ex.LoaderExceptions.Aggregate(msg, (current, e) => current + e!.Message + Environment.NewLine);

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                        catch (Exception ex)
                        {
                            var msg = $"Theme '{theme.FriendlyName}'. {ex.Message}";

                            var fail = new Exception(msg, ex);
                            throw fail;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var msg = string.Empty;
                    for (var e = ex; e != null; e = e.InnerException)
                        msg += e.Message + Environment.NewLine;

                    var fail = new Exception(msg, ex);
                    throw fail;
                }

                ReferencedThemes = referencedThemes;
            }
        }

        #endregion

        #region Utilities

        private static IEnumerable<ThemeInfo> GetThemeInfo()
        {
            if (_themeFolder == null)
                throw new ArgumentNullException(nameof(_themeFolder));

            return (from themeFile in _themeFolder.GetFiles("*.dll", SearchOption.AllDirectories) 
                where IsThemesFolder(themeFile.Directory) select PrepareThemeInfo(themeFile) 
                into theme where theme != null select theme).ToList();
        }

        private static ThemeInfo PrepareThemeInfo(FileInfo themeFile)
        {
            var fileInfo = _config.PluginShadowCopy ? ShadowCopyFile(themeFile, Directory.CreateDirectory(_copyFolder.FullName)) : themeFile;

            Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileInfo.FullName);

            var themeInfo = assembly.GetCustomAttribute<ThemeInfoAttribute>();
            if (themeInfo == null)
            {
                return null;
            }

            var theme = new ThemeInfo
            {
                FriendlyName = themeInfo.FriendlyName,
                SystemName = themeInfo.SystemName,
                Version = themeInfo.Version,
                Author = themeInfo.Author,
                ThemeFileName = fileInfo.Name,
                SupportedVersion = themeInfo.SupportedVersion,
                PreviewText = themeInfo.PreviewText,
                PreviewImageUrl = themeInfo.PreviewImageUrl,
                SupportRtl = themeInfo.SupportRtl,
                Folder = themeInfo.Folder,
                OriginalAssemblyFile = themeFile,
                ReferencedAssembly = assembly
            };

            return theme;
        }
        
        private static FileInfo ShadowCopyFile(FileInfo plug, DirectoryInfo shadowCopyPlugFolder)
        {
            var shouldCopy = true;
            var shadowCopiedPlug = new FileInfo(Path.Combine(shadowCopyPlugFolder.FullName, plug.Name));

            //check if a shadow copied file already exists and if it does, check if it's updated, if not don't copy
            if (shadowCopiedPlug.Exists)
            {
                //it's better to use LastWriteTimeUTC, but not all file systems have this property
                //maybe it is better to compare file hash?
                var areFilesIdentical = shadowCopiedPlug.CreationTimeUtc.Ticks >= plug.CreationTimeUtc.Ticks;
                if (areFilesIdentical)
                {
                    Log.Information("Not copying; files appear identical: {Name}", shadowCopiedPlug.Name);
                    return shadowCopiedPlug;
                }

                //delete an existing file
                Log.Information("New theme found; Deleting the old file: {Name}", shadowCopiedPlug.Name);
                try
                {
                    File.Delete(shadowCopiedPlug.FullName);
                }
                catch (Exception ex)
                {
                    shouldCopy = false;
                    Log.Error(ex, "ThemeManager");
                }
            }

            if (!shouldCopy) return shadowCopiedPlug;
            try
            {
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }
            catch (IOException)
            {
                Log.Information("{FullName} is locked, attempting to rename", shadowCopiedPlug.FullName);
                //this occurs when the files are locked,
                //for some reason devenv locks plugin files some times and for another crazy reason you are allowed to rename them
                //which releases the lock, so that it what we are doing here, once it's renamed, we can re-shadow copy
                try
                {
                    var oldFile = shadowCopiedPlug.FullName + Guid.NewGuid().ToString("N") + ".old";
                    File.Move(shadowCopiedPlug.FullName, oldFile);
                }
                catch (IOException exc)
                {
                    throw new IOException(shadowCopiedPlug.FullName + " rename failed, cannot initialize plugin", exc);
                }
                //ok, we've made it this far, now retry the shadow copy
                File.Copy(plug.FullName, shadowCopiedPlug.FullName, true);
            }

            return shadowCopiedPlug;
        }

        private static void AddApplicationPart(IMvcCoreBuilder mvcCoreBuilder,
            Assembly assembly, string systemName, string filename)
        {
            try
            {
                //we can now register the plugin definition
                Log.Information("Adding to ApplicationParts: '{0}'", systemName);
                mvcCoreBuilder.AddApplicationPart(assembly);

                var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false);
                foreach (var relatedAssembly in relatedAssemblies)
                {
                    var applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                    foreach (var part in applicationPartFactory.GetApplicationParts(relatedAssembly))
                    {
                        mvcCoreBuilder.PartManager.ApplicationParts.Add(part);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ThemeManager");
                throw new InvalidOperationException($"The theme directory for the {systemName} file exists in a folder outside of the allowed grandnode folder hierarchy - exception because of {filename} - exception: {ex.Message}");
            }
        }
        

        /// <summary>
        /// Determines if the folder is a bin plugin folder for a package
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool IsThemesFolder(DirectoryInfo folder)
        {
            if (folder == null) return false;
            if (folder.Name.Equals("bin", StringComparison.InvariantCultureIgnoreCase)) return false;
            return folder.Parent != null && folder.Parent.Name.Equals("Themes", StringComparison.OrdinalIgnoreCase);
        }


        #endregion
    }
}
