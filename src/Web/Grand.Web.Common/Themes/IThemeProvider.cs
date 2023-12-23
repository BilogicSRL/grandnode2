using Grand.Infrastructure.Themes;

namespace Grand.Web.Common.Themes
{
    public interface IThemeProvider
    {
        bool ThemeConfigurationExists(string themeName);

        IList<ThemeConfiguration> GetConfigurations();

        ThemeInfo GetThemeDescriptorFromText(string text);
    }
}
