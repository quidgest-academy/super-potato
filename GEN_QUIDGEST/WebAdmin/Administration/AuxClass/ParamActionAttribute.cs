//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Mvc.Html;

//namespace Administration
//{
//    /// <summary>
//    /// Attribute for controller methods to enable multiplexing the submit action of a form
//    /// </summary>
//    public class HttpParamActionAttribute : ActionNameSelectorAttribute
//    {
//        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
//        {
//            if (actionName.Equals(methodInfo.Name, StringComparison.InvariantCultureIgnoreCase))
//            {
//                return true;
//            }

//            if (!actionName.Equals("Action", StringComparison.InvariantCultureIgnoreCase))
//            {
//                return false;
//            }

//            var request = controllerContext.RequestContext.HttpContext.Request;

//            return request[methodInfo.Name] != null;
//        }
//    }

//}