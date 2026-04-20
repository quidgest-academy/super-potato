using Administration.Models;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CSGenio;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using DbAdmin;
using IConfigurationManager = CSGenio.config.IConfigurationManager;

namespace Administration.Controllers
{
    public class EmailController(IConfigurationManager configManager) : ControllerBase
    {
        private PersistentSupport _sp;
        private PersistentSupport sp
        {
            get
            {
                if(_sp == null)
                    _sp = PersistentSupport.getPersistentSupport(CurrentYear);
                return _sp;
            }
        }

        private User _user;
        private User user
        {
            get
            {
                if(_user == null)
                {
                    _user = SysConfiguration.CreateWebAdminUser(CurrentYear);
                }
                return _user;
            }
        }

        // GET: /Notifications/
        [HttpGet]
        public IActionResult Index()
        {
            //List of email properties configured
            var emailProperties = new List<EmailPropertiesModel>();
            foreach (var emailProperty in CSGenio.framework.Configuration.EmailProperties)
            {
                var emailPropertiesModel = new EmailPropertiesModel();
                emailPropertiesModel.MapFromModel(emailProperty);
                emailProperties.Add(emailPropertiesModel);
            }

            //List of email signatures configured
            var emailSignatures = new List<EmailSignatureModel>();
            foreach (var sp_notificationemailsignature in sp.getEmailSignatures(CurrentYear))
            {
                var emailSignature_viewmodel = new EmailSignatureModel();
                emailSignature_viewmodel.MapFromModel(sp_notificationemailsignature);
                emailSignatures.Add(emailSignature_viewmodel);
            }

            //User registration configuration       
            var userRegistration = CSGenio.framework.Configuration.UserRegistrationEmail;
            var passwordRecovery = CSGenio.framework.Configuration.PasswordRecoveryEmail;

            return Json(new { emailProperties, emailSignatures, userRegistration, passwordRecovery });
        }

        [HttpPost]
        public IActionResult SaveEmail(string userRegistration, string passwordRecovery)
        {
            var conf = configManager.GetExistingConfig();
            conf.UserRegistrationEmail = userRegistration;
            conf.PasswordRecoveryEmail = passwordRecovery;
            configManager.StoreConfig(conf);
			CSGenio.framework.Configuration.ReadConfiguration(conf);
            var ResultMsg = Resources.Resources.FICHEIRO_DE_CONFIGUR18806;
            return Json(new { ResultMsg });
        }



        [HttpGet]
        public IActionResult getEmailSignatureImage(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Json(new { Success = false, Message = "File not found" });            

            CSGenioAnotificationemailsignature nes = CSGenioAnotificationemailsignature.search(sp, key, user, new string[] { "image" });
            if (nes == null)
            {
                return Json(new { Success = false, Message = "File not found" });
            }

            return File(nes.ValImage, "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> setEmailSignatureImage(IFormFile image)
        {
            string[] imageTypes = new string[]{
                "image/gif",
                "image/jpeg",
                "image/pjpeg",
                "image/png"
            };

            //Validade content type
            if(!imageTypes.Contains(image.ContentType))
            {
                return Json(new { Success = false, Message = "Choose a GIF, JPG or PNG image" });
            }

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            string key = FromQuery("key");
            try
            {
                CSGenioAnotificationemailsignature nes = CSGenioAnotificationemailsignature.search(sp, key, user, new string[] { "codsigna", "image" });
                nes.ValImage = bytes;
                nes.ValOpermuda = "WebAdmin";
                nes.ValDatamuda = DateTime.Now;

                sp.openTransaction();
                nes.update(sp);
                sp.closeTransaction();
            }
            catch
            {
                sp.rollbackTransaction();
                return Json(new { Success = false, Message = Resources.Resources.OCORREU_UM_ERRO_AO_P53091 });
            }

            return Json(new { Success = true });
        }

        [HttpGet]
        public IActionResult ManageProperties(string mod, string codpmail)
        {
            EmailServer model;
            if (mod == "1")//new
            {
                model = CSGenio.framework.Configuration.NewEmailServer();
            }
            else
            {
                model = CSGenio.framework.Configuration.EmailProperties.Find(x => x.Codpmail == codpmail);
            }

            var viewmodel = new EmailPropertiesModel();
            viewmodel.MapFromModel(model);
            viewmodel.FormMode = mod;

            return Json(new { Success = true, model = viewmodel });
        }

       
        [HttpPost]
        public IActionResult SaveProperties([FromBody] EmailPropertiesModel modelView)
        {
            
            string resultMessage = "";
            EmailServer model = CSGenio.framework.Configuration.EmailProperties.Find(x => x.Codpmail == modelView.ValCodpmail);
            if (modelView.FormMode == "1")
            {
                model = CSGenio.framework.Configuration.NewEmailServer();
            }
            var conf = configManager.GetExistingConfig();
            try
            {
                //map viewmodel to model
                modelView.MapToModel(model);
               
                switch (modelView.FormMode)
                {
                    case "1":
                        {
                            CSGenio.framework.Configuration.EmailProperties.Add(model);
                            break;
                        }
                    case "3":
                        {
                            CSGenio.framework.Configuration.EmailProperties.Remove(model);
                            break;
                        }
                    default:
                        break;
                }

                conf.EmailProperties = CSGenio.framework.Configuration.EmailProperties;
                configManager.StoreConfig(conf);
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                resultMessage = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
            }

            return Json(new { Success = false, ResultMsg = resultMessage });
        }

        

        public IActionResult Send(string idnotif)
        {
            var viewModel = (Notification)PersistentSupport.getNotifications()[idnotif];
            viewModel.Run(sp, user);
            return Ok();
        }

        public IActionResult SendAll()
        {
            var notifications = PersistentSupport.getNotifications();
            foreach (Notification notification in notifications.Values)
                notification.Run(sp, user);
            return Ok();
        }

        [HttpGet]
        public IActionResult ManageSignature(string mod, string codsigna)
        {
            var model = CSGenioAnotificationemailsignature.search(sp, codsigna, user);
            if (mod == "1")//new
            {
                if(model == null)
                {
                    model = new CSGenioAnotificationemailsignature(user)
                    {
                        ValOpercria = "WebAdmin",
                        ValDatacria = DateTime.Now
                    };
                    
                    try
                    {
                        sp.openTransaction();
                        model.insert(sp);
                        sp.closeTransaction();
                    }
                    catch
                    {
                        sp.rollbackTransaction();
                        return Json(new { Success = false, Message = Resources.Resources.OCORREU_UM_ERRO_AO_P53091 });
                    }
                }
            }

            var viewmodel = new EmailSignatureModel();
            viewmodel.MapFromModel(model);
            viewmodel.FormMode = mod;

            return Json(new { Success = true, model = viewmodel });
        }

        [HttpPost]
        public IActionResult SaveSignature([FromBody] EmailSignatureModel modelView)
        {
            sp.openConnection();
            CSGenioAnotificationemailsignature model = CSGenioAnotificationemailsignature.search(sp, modelView.ValCodsigna, user);
            if (modelView.FormMode == "1" || model == null)
            {
                model = new CSGenioAnotificationemailsignature(user)
                {
                    ValOpercria = "WebAdmin",
                    ValDatacria = DateTime.Now
                };
            }
            else if (modelView.FormMode == "2" && model != null)
            {
                model.ValOpermuda = "WebAdmin";
                model.ValDatamuda = DateTime.Now;
            }
            
            try
            {
                //map viewmodel to model
                modelView.MapToModel(model);

                
                switch (modelView.FormMode)
                {
                    case "1":
                    case "2":
                        {
                            model.update(sp);
                            break;
                        }
                    case "3":
                        {
                            model.delete(sp);
                            break;
                        }
                    default:
                        break;
                }

                sp.closeConnection();
                return Json(new { Success = true });
            }
            catch (Exception e)
            {
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                return Json(new { Success = false, model });
            }
        }
    }
}