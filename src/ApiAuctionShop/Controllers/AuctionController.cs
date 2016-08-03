
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    //kontroler dla aukcji 
    public class AuctionController : Controller
    {
        public ApplicationDbContext _context;
        private readonly UserManager<Signup> _userManager;

        public AuctionController(
            UserManager<Signup> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [AllowAnonymous]
        public ActionResult AuctionPage(int id)
        {
            return View(GetAuction(id));
        }

        //zmienic nazwe na AuctionLists
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AuctionList()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());

            var list_mine = _context.Auctions.Where(d => d.SignupId == user.Id).ToList();
            //w perpektywie: nie wszystkie, tylko trwające
            var list_all = _context.Auctions.ToList();
            List<List<Auctions>> lists = new List<List<Auctions>>();
            lists.Add(list_mine);
            lists.Add(list_all);

            //var list = _context.Auctions.ToList();
            return View(lists);
        }

        //////////////////TEST /////////////////////////
        [Authorize]
        public IActionResult AddAuction()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddAuction(Auctions auction, IFormFile file = null)
        {
            if (file != null)
            {
                if (file.ContentType.Contains("image"))
                {
                    using (var fileStream = file.OpenReadStream())
                    {
                        using (var ms = new MemoryStream())
                        {
                            using (var imageFactory = new ImageFactory())
                            {
                                imageFactory.FixGamma = false;
                                imageFactory.Load(fileStream).Resize(new ResizeLayer(new Size(400, 400), ResizeMode.Stretch))
                                .Format(new JpegFormat { })
                                .Quality(100)
                                .Save(ms);
                            }

                            var fileBytes = ms.ToArray();

                            var _auction = new Auctions()
                            {
                                title = auction.title,
                                description = auction.description,
                                duration = auction.duration,
                                price = auction.price,
                                ImageData = fileBytes
                            };

                            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());

                            user.Auction.Add(_auction);

                            var result = await _userManager.UpdateAsync(user);

                            if (result.Succeeded)
                            {
                                return RedirectToAction("AuctionList", "Auction");
                            }
                        }
                    }
                }
            }
            return RedirectToAction("AuctionList", "Auction");
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
