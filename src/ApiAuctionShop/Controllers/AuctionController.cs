
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    //kontroler dla aukcji 
    public class AuctionController : Controller
    {
        public ApplicationDbContext _context;

        public AuctionController(ApplicationDbContext context)
        {
            _context = context;
        }


        [AllowAnonymous]
        public ActionResult AuctionPage(int id)
        {
            return View(GetAuction(id));
        }



        [Authorize]
        [HttpGet]
        public Auctions GetAuction(int id)
        {
          
            //var list = _context.Auctions.ToList();
            return (Auctions) _context.Auctions.Where(d => d.ID == id).ToList()[0];
        }
    }

}
