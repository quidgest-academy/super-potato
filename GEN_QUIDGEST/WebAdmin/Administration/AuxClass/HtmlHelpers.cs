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

//namespace Administration.Helpers
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

//    public static class HtmlHelpers
//    {
//        private static string flattenHtmlProps(IDictionary<string, object> htmlProperties)
//        {
//            StringBuilder line = new StringBuilder();
//            foreach (var pair in htmlProperties)
//                line.Append(" " + pair.Key + "='" + pair.Value + "'");
//            return line.ToString();
//        }

//        /// <summary>
//        /// Helper to display for the given expression even if it is null or empty
//        /// </summary>
//        /// <param name="html">The HTML Helper</param>
//        /// <param name="expression">The expression</param>
//        /// <returns>The MVCHtmlString to be rendered by the template</returns>
//        public static MvcHtmlString DisplayForWithNull<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Object htmlProperties = null)
//        {
//            if (htmlProperties == null)
//                htmlProperties = new { };

//            IDictionary<string, object> htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlProperties);

//            var htmlAttr = flattenHtmlProps(htmlAttributes);

//            MvcHtmlString display = DisplayExtensions.DisplayFor(html, expression, htmlAttributes);
//            if (display.ToString() == MvcHtmlString.Empty.ToString())
//                return MvcHtmlString.Create("<text class='empty-value'" + htmlAttr + ">&lt;Empty&gt;</text>");
//            return display;
//        }


//        private static string GetEnumDisplayName<TEnum>(object item)
//        {
//            string res = item.ToString();
//            var da = ((DisplayAttribute)(typeof(TEnum).GetField(res).GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault()));
//            return da == null ? res : da.GetName();
//        }

//        public static IEnumerable<SelectListItem> ToSelectList<TEnum>(object selected)
//        {
//            return
//            from v in (TEnum[])(Enum.GetValues(typeof(TEnum)))
//            select new SelectListItem()
//            {
//                Text = GetEnumDisplayName<TEnum>(v),
//                Value = v.ToString(),
//                Selected = (v.ToString() == selected.ToString())
//            };
//        }

//    }
//}