using Administration.AuxClass;
using CSGenio;
using CSGenio.framework;
using GenioServer.framework;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;

namespace Administration.Controllers
{
    public class ConfigMigrationController(CSGenio.config.IConfigurationManager configManager) : ControllerBase
    {
        private IActionResult startPage(Models.ConfigMigrationModel model, bool redirect)
        {

            if (!AuxFunctions.CheckXMLIsValid(configManager))
            {
                model.ResultMsg = Resources.Resources.E_NECESSARIO_PROCEDE36325;
                redirect = false;
            }
            model.ConfigVersion = AuxFunctions.GetConfigVersion(configManager);

            return Json(new { model, redirect });
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new Models.ConfigMigrationModel();
            return startPage(model, false);
        }

        [HttpPost]
        public IActionResult MigrateConfig([FromBody] Models.ConfigMigrationModel model)
        {
            var configVersion = AuxFunctions.GetConfigVersion(configManager);
            ConfigXMLMigration.Migration(configManager, configVersion);

            //reload configuration file         
            Configuration.Reload();

            return startPage(model, true);
        }

        
    }
}
