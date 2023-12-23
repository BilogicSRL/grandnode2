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

        public bool ThemeConfigurationExists(string themeName)
        {
            return GetConfigurations().Any(configuration => configuration.Name.Equals(themeName, StringComparison.OrdinalIgnoreCase));
        }

        public IList<ThemeConfiguration> GetConfigurations()
        {
            return _themeList.ThemeConfigurations;
        }

        public ThemeInfo GetThemeDescriptorFromText(string text)
        {
            var themeDescriptor = new ThemeInfo();
            try
            {
                var themeConfiguration = JsonConvert.DeserializeObject<ThemeConfiguration>(text);
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
