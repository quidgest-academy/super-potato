using CSGenio.framework;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
    public class MainController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }

        [HttpGet]
        public IActionResult GetYears()
        {
            return Ok(new { CSGenio.framework.Configuration.DefaultYear, CSGenio.framework.Configuration.Years });
        }

        [HttpGet]
        public IActionResult GetApplications()
        {
            return Ok(new { ClientApplication.Applications });
        }


        [HttpGet]
        public IActionResult GetGlobalSettingsJson()
        {
            var defaultSystem = string.Empty;
            var Years = new List<string>();
            try
            {
                defaultSystem = CSGenio.framework.Configuration.DefaultYear;
                Years = CSGenio.framework.Configuration.Years;
            }
            catch { 
                /* Catch erro related with "file not found"
                Set "0" because it's the most common and Vue doesn't accept empty
                 */ 
                defaultSystem = "0";    
            }

            return Ok(new
            {
                activeLog = false,
                defaultSystem,
                defaultLang = "en-US",
                Years,
                ClientApplication.Applications
            });
        }
		
    }
}
