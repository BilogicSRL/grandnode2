using Grand.SharedKernel.Attributes;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Grand.Web.Controllers
{
    public class HomeController : BasePublicController
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }

        [IgnoreApi]
        [HttpGet]
        public virtual IActionResult Index()
        {
            logger.LogTrace("ILogger Trace from HomeController()");
            logger.LogDebug("ILogger Debug from HomeController()");
            logger.LogInformation("ILogger Info from HomeController()");
            logger.LogWarning("ILogger Warn from HomeController()");
            logger.LogError("ILogger Error from HomeController()");
            logger.LogCritical("ILogger Fatal from HomeController()");
            return View();
        }
    }
}
