using Grand.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Grand.Infrastructure.Themes
{
    public sealed class ThemeInfo
    {
        public ThemeInfo(
            FileInfo originalAssemblyFile,
            Assembly referencedAssembly)
            : this()
        {
            ReferencedAssembly = referencedAssembly;
            OriginalAssemblyFile = originalAssemblyFile;
        }
        public ThemeInfo()
        {
        }

        /// <summary>
        /// The assembly that has been shadow copied that is active in the application
        /// </summary>
        public Assembly ReferencedAssembly { get; internal set; }

        /// <summary>
        /// The original assembly file that a shadow copy was made from it
        /// </summary>
        public FileInfo OriginalAssemblyFile { get; internal set; }

        public string ThemeFileName { get; set; }
        
        /// <summary>
        /// Gets or sets the theme system name
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the theme friendly name
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// Gets or sets the folder theme name
        /// </summary>
        public string Folder { get; set; }
        
        /// <summary>
        /// Gets or sets the author of theme
        /// </summary>
        public string Author { get; set; }
        
        public string SupportedVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the theme system name
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the theme supports RTL (right-to-left)
        /// </summary>
        public bool SupportRtl { get; set; }

        /// <summary>
        /// Gets or sets the path to the preview image of the theme
        /// </summary>
        public string PreviewImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the preview text of the theme
        /// </summary>
        public string PreviewText { get; set; }
    }
}
