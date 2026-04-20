using Administration.Models;
using CSGenio;
using CSGenio.business;
using CSGenio.framework;
using CSGenio.persistence;
using Quidgest.Persistence;
using Quidgest.Persistence.GenericQuery;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Administration.AuxClass;
using DbAdmin;

namespace Administration.Controllers
{
    public class NotificationsController : ControllerBase
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
            var notifications = new List<NotificationModel>();

            //List of Genio Notifications
            var notifs = PersistentSupport.getNotifications();
            if (notifs != null)
            {
                foreach (Notification notification in PersistentSupport.getNotifications().Values)
                {
                    var MesgsArea = "notificationmessage";
                    notification.MessagesConfig = CSGenioAnotificationmessage.searchList(sp, user, CriteriaSet.And().Equal(new Quidgest.Persistence.FieldRef(MesgsArea, "codtpnot"), notification.codtpnot).Equal(new Quidgest.Persistence.FieldRef(MesgsArea, "zzstate"), 0));

                    var notificationModel = new NotificationModel();
                    notificationModel.MapFromModel(notification);
                    notifications.Add(notificationModel);
                }
            }

            return Json(new { notifications});
        }


        [HttpGet]
        public IActionResult ManageNotif(string mod, string idnotif)
        {
            Notification sp_NotificationModel = (Notification)PersistentSupport.getNotifications()[idnotif];

            if (!String.IsNullOrEmpty(sp_NotificationModel.DatabaseFieldMapping.MessagesTable))
            {
                String area_lower = sp_NotificationModel.DatabaseFieldMapping.MessagesTable.ToLower();
                Type type = Type.GetType("CSGenio.business.CSGenioA" + area_lower + ", GenioServer");
                System.Reflection.MethodInfo methodInfo = type.GetMethod("searchList", new[] { typeof(PersistentSupport), typeof(User), typeof(CriteriaSet), typeof(string[]), typeof(bool), typeof(bool) });
                var result = methodInfo.Invoke(null, new object[] { sp, user, CriteriaSet.And().Equal(new Quidgest.Persistence.FieldRef(area_lower, "zzstate"), 0).Equal(new Quidgest.Persistence.FieldRef(area_lower, "idnotif"), idnotif), null, false, false });

                sp_NotificationModel.NotifsOnBD.Clear();
                if (result != null)
                {
                    List<object> listAreas = new List<object>((IEnumerable<Object>)result);
                    int total = listAreas.Count;
                    for (int i = 0; i < total; i++)
                    {
                        DbArea db = (DbArea)listAreas[i];
                        sp_NotificationModel.NotifsOnBD.Add(db);
                    }
                }
            }
            String MesgsArea = "notificationmessage";
            sp_NotificationModel.MessagesConfig = CSGenioAnotificationmessage.searchList(sp, user, CriteriaSet.And().Equal(new Quidgest.Persistence.FieldRef(MesgsArea, "codtpnot"), sp_NotificationModel.codtpnot).Equal(new Quidgest.Persistence.FieldRef(MesgsArea, "zzstate"), 0));


            var notificationModel = new NotificationModel();
            notificationModel.MapFromModel(sp_NotificationModel);

            return Json(new { Success = true, model = notificationModel });
        }

        [HttpGet]
		public IActionResult ManageMessage(string mod, string codmesgs, string idnotif)
        {
            
            Notification viewModel = (Notification)PersistentSupport.getNotifications()[idnotif];
            String codtpnot = viewModel.codtpnot;
            
            CSGenioAnotificationmessage model = null;
            if (mod == "1")//new
            {
                model = new CSGenioAnotificationmessage(user)
                {
                    ValCodmesgs = Guid.NewGuid().ToString(),
                    ValOpercria = "WebAdmin",
                    ValDatacria = DateTime.Now
                };
            }
			else
				model = CSGenioAnotificationmessage.search(sp, codmesgs, user);
                
            model.ValCodtpnot = codtpnot;
            model.ValIdnotif = idnotif;

            NotificationMessageModel viewmodel = new NotificationMessageModel();
            viewmodel.MapFromModel(model);
            viewmodel.FormMode = mod;


            //Email properties list fill:
            var emailProps = CSGenio.framework.Configuration.EmailProperties;

            viewmodel.TableEmailProperties = new SelectList(emailProps.Select(ep => new SelectListItem() { Text = ep.Id, Value = ep.Codpmail.ToUpper(), Selected = (ep.Codpmail == viewmodel.ValCodpmail) }),
                "Value", "Text",
                viewmodel.ValCodpmail);

            //Allowed Destinations list fill:
            var allowedDestinations = viewModel.AllowedDestinations;

            viewmodel.TableAllowedDestinations = new SelectList(allowedDestinations.Select(destn => new SelectListItem() { Text = destn.Destination.DestinationName, Value = destn.Destination.DestinationKey.ToString().ToUpper(), Selected = (destn.Destination.DestinationKey == viewmodel.ValCoddestn) }),
                "Value", "Text",
                viewmodel.ValCoddestn);

            //Allowed Tags list fill:
            viewmodel.TableAllowedTags = new SelectList(viewModel.TagsFieldMapping.Select(tag => new SelectListItem() { Text = tag.FieldMap.FieldnameApp, Value = tag.FieldMap.FieldnameApp.ToUpper(), Selected = (tag.FieldMap.FieldnameApp == viewmodel.ValSelectedTag) }),
                "Value", "Text",
                viewmodel.ValSelectedTag);

            //Email signatures list fill:
            var emailSignatures = CSGenioAnotificationemailsignature.searchList(sp, user, CriteriaSet.And().Equal(new FieldRef("notificationemailsignature", "zzstate"), 0));

            viewmodel.TableEmailSignatures = new SelectList(emailSignatures.Select(ep => new SelectListItem() { Text = (string)ep.returnValueField("notificationemailsignature.name"), Value = ((string)ep.returnValueField("notificationemailsignature.codsigna")).ToUpper(), Selected = ((string)ep.returnValueField("notificationemailsignature.codsigna") == viewmodel.ValCodsigna) }),
                "Value", "Text",
                viewmodel.ValCodsigna);

            return Json(new { Success = true, model = viewmodel });
        }
        
       [HttpPost]
        public IActionResult SaveMessage([FromBody] NotificationMessageModel modelView)
        {
            sp.openConnection();
            CSGenioAnotificationmessage model = CSGenioAnotificationmessage.search(sp, modelView.ValCodmesgs, user);
            if (modelView.FormMode == "1")
            {
                model = new CSGenioAnotificationmessage(user)
                {
                    ValOpercria = "WebAdmin",
                    ValDatacria = DateTime.Now
                };
            }
            try
            {
                //map viewmodel to model
                modelView.MapToModel(model);
                Notification notification = (Notification)PersistentSupport.getNotifications()[model.ValIdnotif];

                //Email properties
                if (!String.IsNullOrEmpty(model.ValCodpmail))
                {
                    EmailServer emailProps = CSGenio.framework.Configuration.EmailProperties.Find(x => x.Codpmail.ToUpper() == model.ValCodpmail.ToUpper());
                    //formula + manual
                    model.ValFrom = emailProps.From;
                }

                //Destination
                if (!String.IsNullOrEmpty(model.ValCoddestn))
                {
                    Notification.AllowedDestination destination = notification.AllowedDestinations.Find(x => x.DestinationKey.ToUpper() == model.ValCoddestn.ToUpper());
                    //formula + manual
                    model.ValTo = destination.Destination.DestinationName;
                }

                switch (modelView.FormMode)
                {
                    case "1":
                        {
                            model.insert(sp);
                            break;
                        }
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

                return Json(new { Success = true  });
            }
            catch (Exception e)
            {
                model.ResultMsg = Translations.Get(e.Message, CultureInfo.CurrentCulture.Name.Replace("-", "").ToUpper());
                return Json(new { Success = false, model  });
            }

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
       
    }
}