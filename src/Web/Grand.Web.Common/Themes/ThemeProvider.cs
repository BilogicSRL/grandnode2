using Grand.Infrastructure.Themes;
using Grand.SharedKernel.Extensions;
using Newtonsoft.Json;

namespace Grand.Web.Common.Themes
{
    public class ThemeProvider : IThemeProvider
    {
        #region Fields

        private readonly IThemeList _themeList;

        #endregion

        #region Constructors

        public ThemeProvider(IThemeList themeList)
        {
            _themeList = themeList;
        }

        #endregion

        #region Methods

        public bool ThemeConfigurationExists(string themeName, out Theme theme)
        {
            theme = GetConfigurations().FirstOrDefault(configuration => configuration.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));
            return theme != null;
        }

        public IList<Theme> GetConfigurations()
        {
            return _themeList.ThemeConfigurations;
        }

        public ThemeInfo GetThemeDescriptorFromText(string text)
        {
            var themeDescriptor = new ThemeInfo();
            try
            {
                var themeConfiguration = JsonConvert.DeserializeObject<Theme>(text);
                themeDescriptor.FriendlyName = themeConfiguration?.Title;
            }
            catch
            {
                // ignored
            }

            return themeDescriptor;
        }

        #endregion
    }
}
