﻿using Grand.Domain.Stores;
using Microsoft.AspNetCore.Http;

namespace Grand.Web.Common.Themes;

public class ThemeContext : ThemeContextBase
{
    private readonly StoreInformationSettings _storeInformationSettings;
    private readonly IHttpContextAccessor _contextAccessor;

    public ThemeContext(IHttpContextAccessor contextAccessor, StoreInformationSettings storeInformationSettings) :
        base(contextAccessor)
    {
        _storeInformationSettings = storeInformationSettings;
        _contextAccessor = contextAccessor;
    }

    public override string AreaName => "";

    public override string GetCurrentTheme()
    {
        var theme = "";
        if (_storeInformationSettings.AllowCustomerToSelectTheme)
        {
            theme = _contextAccessor.HttpContext?.Session.GetString(this.SessionName);
        }
        //default store theme
        if (string.IsNullOrEmpty(theme))
            theme = _storeInformationSettings.DefaultStoreTheme;

        return theme;
    }
}