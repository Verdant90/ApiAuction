using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Identity;
using ApiAuctionShop.Database;
using System.Threading;
using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.AspNet.Mvc.Localization;

namespace Projekt.Controllers
{
    public class HomeController : Controller
    {
        readonly IHtmlLocalizer<HomeController> _localizer;
        public ApplicationDbContext _context;
        private readonly UserManager<Signup> _userManager;
        public HomeController(
           UserManager<Signup> userManager, ApplicationDbContext context, IHtmlLocalizer<HomeController> localizer)
        {
            _userManager = userManager;
            _context = context;
            _localizer = localizer;
        }
        
        // zwraca strone glowna
        public ActionResult Index(string language)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(language);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            AdminSettingsViewModel model = new AdminSettingsViewModel();
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            model.startMessage = settings.startMessage;
            return View(model);
        }

    }
}
