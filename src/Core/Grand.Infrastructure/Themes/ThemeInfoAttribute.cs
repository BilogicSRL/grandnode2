using System.Reflection;

namespace Grand.Infrastructure.Themes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ThemeInfoAttribute : Attribute
    {
        public ThemeInfoAttribute()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version fullVersion = assembly.GetName().Version;
            SupportedVersion = $"{fullVersion?.Minor}.{fullVersion?.Major}";
            
        }
        
        /// <summary>
        /// A string property to set or get a user-friendly name for the theme
        /// </summary>
        public string FriendlyName { get; set; } = string.Empty;
        /// <summary>
        /// A string property for the system name of the theme
        /// </summary>
        
        public string SystemName { get; set; } = string.Empty;
        /// <summary>
        /// A string property indicating the folder where the theme is located in Views
        /// </summary>
        public string Folder { get; set; } = string.Empty;
        
        /// <summary>
        /// A string property for the author's name.
        /// </summary>
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// A string property indicating the supported version of the theme, set by the constructor.
        /// </summary>
        public string SupportedVersion { get; set; }

        /// <summary>
        /// A string property for the theme's version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// A boolean property indicating whether the theme supports Right-to-Left (RTL) text.
        /// </summary>
        public bool SupportRtl { get; set; } 
        
        /// <summary>
        /// A string property for the URL of an image preview of the theme
        /// </summary>
        public string PreviewImageUrl { get; set; }= string.Empty;
        
        /// <summary>
        /// A string property for a textual preview or description of the theme
        /// </summary>
        public string PreviewText { get; set; }= string.Empty;
    }
}