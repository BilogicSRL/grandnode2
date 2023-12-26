using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Infrastructure;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Stores;
using Grand.Domain.Vendors;

namespace Grand.Web.Common.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IUserFieldService _userFieldService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IThemeProvider _themeProvider;

        private string _cachedAdminThemeName;
        private Theme _cachedTheme;

        public ThemeContext(IWorkContext workContext,
            IUserFieldService userFieldService,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings,
            IThemeProvider themeProvider)
        {
            _workContext = workContext;
            _userFieldService = userFieldService;
            _storeInformationSettings = storeInformationSettings;
            _vendorSettings = vendorSettings;
            _themeProvider = themeProvider;
        }

        /// <summary>
        /// Get current theme system name
        /// </summary>
        public Theme WorkingTheme {
            get {
                if (_cachedTheme != null)
                    return _cachedTheme;

                var theme = "";
                if (_storeInformationSettings.AllowCustomerToSelectTheme)
                {
                    if (_workContext.CurrentCustomer != null)
                        theme = _workContext.CurrentCustomer.GetUserFieldFromEntity<string>(
                            SystemCustomerFieldNames.WorkingThemeName, _workContext.CurrentStore.Id);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _storeInformationSettings.DefaultStoreTheme;

                if (_themeProvider.ThemeConfigurationExists(theme, out _cachedTheme)) return _cachedTheme;
                
                var themeInstance = _themeProvider.GetConfigurations()
                    .FirstOrDefault();
                _cachedTheme = themeInstance ?? throw new Exception("No theme could be loaded");

                return _cachedTheme;
            }
        }

        /// <summary>
        /// Get current theme system name
        /// </summary>
        public string AdminAreaThemeName {
            get {
                if (!string.IsNullOrEmpty(_cachedAdminThemeName))
                    return _cachedAdminThemeName;

                var theme = "Default";

                if (!string.IsNullOrEmpty(_workContext.CurrentStore.DefaultAdminTheme))
                    theme = _workContext.CurrentStore.DefaultAdminTheme;

                if (_storeInformationSettings.AllowToSelectAdminTheme)
                {
                    if (_workContext.CurrentCustomer != null)
                    {
                        var customerTheme =
                            _workContext.CurrentCustomer.GetUserFieldFromEntity<string>(
                                SystemCustomerFieldNames.AdminThemeName, _workContext.CurrentStore.Id);
                        if (!string.IsNullOrEmpty(customerTheme))
                            theme = customerTheme;
                    }
                }

                if (_workContext.CurrentVendor != null)
                {
                    if (!string.IsNullOrEmpty(_vendorSettings.DefaultAdminTheme))
                        theme = _vendorSettings.DefaultAdminTheme;
                }

                //cache theme
                _cachedAdminThemeName = theme;
                return theme;
            }
        }

        /// <summary>
        /// Set current theme system name
        /// </summary>
        /// <param name="themeName"></param>
        /// <returns></returns>
        public virtual async Task SetWorkingTheme(string themeName)
        {
            if (!_storeInformationSettings.AllowCustomerToSelectTheme)
                return;

            if (_workContext.CurrentCustomer == null)
                return;

            await _userFieldService.SaveField(_workContext.CurrentCustomer, SystemCustomerFieldNames.WorkingThemeName,
                themeName, _workContext.CurrentStore.Id);
            
        }
    }
}