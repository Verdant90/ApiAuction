using ApiAuctionShop.Helpers;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Projekt.Controllers;
using System;
using System.Linq;
using System.Text;

namespace ApiAuctionShop.Attributes
{
    public class LoginAuthentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
           var context = ((MobileauctionController)filterContext.Controller).context;

            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];

            if (!string.IsNullOrEmpty(auth))
            {
                int i = auth.ToString().IndexOf("Basic ");
                string code = auth.ToString().Substring(i + "Basic ".Length);

                var token = Encoding.Default.GetString(Convert.FromBase64String(code));
                var encrypt = StringCipher.Decrypt(token, Settings.HashPassword);

                var myEmail = context.Logins.Where(s => s.Email == encrypt[0]);

                if (myEmail.Any() && StringCipher.IsExpiredToken(encrypt[1]) && myEmail.First().IsTokenConfirmed == true && myEmail.First().Token == code) 
                {
                    return;
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = "Wrong Email" };
                    return;
                }

            }
            filterContext.Result = new ContentResult { Content = "Error 403" };
        }
    }
}
