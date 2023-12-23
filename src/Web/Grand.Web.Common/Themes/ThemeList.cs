using Grand.Infrastructure.Themes;
using Grand.SharedKernel.Extensions;
using Newtonsoft.Json;

namespace Grand.Web.Common.Themes
{
    public class ThemeList : IThemeList
    {
        public ThemeList()
        {
            ThemeConfigurations = new List<ThemeConfiguration>();
            ThemeManager.ReferencedThemes.ToList().ForEach(theme =>
            {
                var configuration = CreateThemeConfiguration(theme);
                if (configuration != null)
                {
                    ThemeConfigurations.Add(configuration);
                }
            });
        }

        public IList<ThemeConfiguration> ThemeConfigurations { get; }

        private static ThemeConfiguration CreateThemeConfiguration(ThemeInfo theme)
        {
            return new ThemeConfiguration {
                Name = theme.SystemName,
                Title = theme.FriendlyName,
                PreviewText = theme.PreviewText,
                SupportRtl = theme.SupportRtl,
                Version = theme.Version,
                PreviewImageUrl = theme.PreviewImageUrl
            };
        }

    }
}
