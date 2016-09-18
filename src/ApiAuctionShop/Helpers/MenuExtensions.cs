using Microsoft.AspNet.Html.Abstractions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.WebEncoders;
using System;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Helpers
{
    public static class MenuExtensions
    {
        public static string GetRequiredString(this RouteData routeData, string keyName)
        {
            object value;
            if (!routeData.Values.TryGetValue(keyName, out value))
            {
                throw new InvalidOperationException($"Could not find key with name '{keyName}'");
            }

            return value?.ToString();
        }

        public static string GetString(IHtmlContent content)
        {
            var writer = new System.IO.StringWriter();
            content.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static HtmlString MenuItem(
            this IHtmlHelper htmlHelper,
            string text,
            string action,
            string controller,
            string liCssClass = null
        )
        {
            var li = new TagBuilder("li");
            if (!String.IsNullOrEmpty(liCssClass))
            {
                li.AddCssClass(liCssClass);
            }
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase))
            {
                li.InnerHtml.AppendHtml(String.Format("<li class=\"active\"><a href=\"/AdminPanel/{0}\"><i class=\"glyphicon glyphicon-chevron-right\"></i>{1}</a></li>",
                controller, text));
                li.AddCssClass("active");
            }
            else
            {
                li.InnerHtml.AppendHtml(String.Format("<li><a href=\"/AdminPanel/{0}\"><i class=\"glyphicon glyphicon-chevron-right\"></i>{1}</a></li>",
                controller, text));
            }
            //li.InnerHtml.AppendHtml(String.Format("<a href=\"@Url.Action({0}, {1})\"><i class=\"glyphicon glyphicon-chevron-right\"></i>{2}</a>",
            //   action, controller, text));
            //li.InnerHtml.AppendHtml(String.Format("<a href=\"/AdminPanel/{0}\"><i class=\"glyphicon glyphicon-chevron-right\"></i>{1}</a>",
            //    controller, text));
            return new HtmlString(GetString( li.InnerHtml));
        }
    }
}
