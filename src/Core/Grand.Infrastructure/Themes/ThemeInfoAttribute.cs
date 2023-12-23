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
        public string FriendlyName { get; set; } = string.Empty;
        public string SystemName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public string SupportedVersion { get; set; }

        public string Version { get; set; }

        public bool SupportRtl { get; set; } 
        public string PreviewImageUrl { get; set; }= string.Empty;
        public string PreviewText { get; set; }= string.Empty;
    }
}