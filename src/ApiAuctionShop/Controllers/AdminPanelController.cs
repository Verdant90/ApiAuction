using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Controllers
{
    public class AdminPanelController : Controller
    {
        // GET: /AdminPanel/
        public IActionResult AdminPanel()
        {
            return View("Index");
        }
    }
}
