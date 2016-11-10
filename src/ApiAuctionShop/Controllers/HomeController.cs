using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Identity;
using ApiAuctionShop.Database;

namespace Projekt.Controllers
{
    public class HomeController : Controller
    {

        public ApplicationDbContext _context;
        private readonly UserManager<Signup> _userManager;
        public HomeController(
           UserManager<Signup> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }

        public ActionResult Index()
        {

            AdminSettingsViewModel model = new AdminSettingsViewModel();
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            model.startMessage = settings.startMessage;
            return View(model);
        }

    }
}
