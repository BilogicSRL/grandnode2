using Grand.Infrastructure.Themes;

namespace Grand.Web.Common.Themes
{
    public interface IThemeProvider
    {
        bool ThemeConfigurationExists(string themeName, out Theme theme);

        IList<Theme> GetConfigurations();

        ThemeInfo GetThemeDescriptorFromText(string text);
    }
}
