using System.Linq;
using Microsoft.AspNet.Mvc;
using System.Threading;
using System.Globalization;
using System.Web;

namespace ApiAuctionShop.Controllers
{
    public class LanguageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // zmiana jezyka
        public ActionResult Change(string LanguageAbbrevation)
        {
            if(LanguageAbbrevation != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(LanguageAbbrevation);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(LanguageAbbrevation);

            }
            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = LanguageAbbrevation;
            Response.Cookies.Append("Language", LanguageAbbrevation);

            return View("Index");
        }
    }
}