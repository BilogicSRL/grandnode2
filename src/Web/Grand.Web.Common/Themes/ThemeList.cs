using Grand.Infrastructure.Themes;
using Grand.SharedKernel.Extensions;
using Newtonsoft.Json;

namespace Grand.Web.Common.Themes
{
    public class ThemeList : IThemeList
    {
        public ThemeList()
        {
            ThemeConfigurations = new List<Theme>();
            ThemeManager.ReferencedThemes.ToList().ForEach(theme =>
            {
                var configuration = CreateThemeConfiguration(theme);
                if (configuration != null)
                {
                    ThemeConfigurations.Add(configuration);
                }
            });
        }

        public IList<Theme> ThemeConfigurations { get; }

        private static Theme CreateThemeConfiguration(ThemeInfo theme)
        {
            return new Theme {
                Name = theme.SystemName,
                Title = theme.FriendlyName,
                PreviewText = theme.PreviewText,
                SupportRtl = theme.SupportRtl,
                Version = theme.Version,
                Folder = theme.Folder,
                PreviewImageUrl = theme.PreviewImageUrl
            };
        }

    }
}
