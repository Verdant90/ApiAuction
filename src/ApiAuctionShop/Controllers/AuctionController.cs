
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == id);

            BiddingViewModel bvm = new BiddingViewModel
            {
                auctionToSend = tmp,
                bid = -1
            };

            return View(bvm);
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
            TryValidateModel(auction);
            if (!ModelState.IsValid)
            {
                return View();
            }
            DateTime myDateTime = DateTime.Now;
            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var _auction = new Auctions()
            {
                title = auction.title,
                description = auction.description,
                startDate = DateTime.Parse(auction.startDate).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                endDate = DateTime.Parse(auction.endDate).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                startPrice = auction.startPrice,
                buyPrice = auction.buyPrice,
                author = auction.author
                //currentPrice = (decimal)auction.price,
            };
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
                            _auction.ImageData = fileBytes;
                        }
                    }
                 }
            }
            if (DateTime.Parse(_auction.startDate) <= DateTime.Now) _auction.state = "active";
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());

            user.Auction.Add(_auction);

            var result = await _userManager.UpdateAsync(user);

            
            return RedirectToAction("AuctionList", "Auction");
        }

        // GET: /Movies/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {/*
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            */
            Auctions auctionToEdit = _context.Auctions.First(i => i.ID == id);

            if (auctionToEdit == null)
            {
                return HttpNotFound();
            }
            return View(auctionToEdit);
        }
        
        // POST: /Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Auctions auction, IFormFile file = null)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());

                
                var tmp = _context.Auctions.FirstOrDefault(i => i.ID == auction.ID);
                if(tmp != null)
                {
                    if(tmp.state == "waiting")
                    {
                        tmp.title = auction.title;
                        tmp.description = auction.description;
                        tmp.buyPrice = auction.buyPrice;
                        tmp.endDate = auction.endDate;
                        tmp.startPrice = auction.startPrice;
                        tmp.startDate = auction.startDate;
                    }else if (tmp.state == "active")
                    {
                        tmp.endDate = auction.endDate;
                    }

                }
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
                                tmp.ImageData = fileBytes;
                            }
                        }
                    }
                }

                if (DateTime.Parse(tmp.startDate) <= DateTime.Now) tmp.state = "active";
                if (DateTime.Parse(tmp.endDate) <= DateTime.Now) tmp.state = "ended";
                _context.SaveChanges();
            }
            return RedirectToAction("AuctionList", "Auction");
        }


        // GET: /Auction/AddBid/5
        [Authorize]
        public ActionResult AddBid(int? id)
        {
            Auctions auctionToBid = _context.Auctions.First(i => i.ID == id);

            if (auctionToBid == null)
            {
                return HttpNotFound();
            }
            return View(auctionToBid);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBid(Auctions au)
        {
            //auction's Id is not set here :/
            Bid newBid = new Bid();
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            newBid.bid = decimal.Parse(au.bid);
            newBid.bidAuthor = user.Email;
            newBid.bidDate = DateTime.Now;
            _context.Auctions.FirstOrDefault(i => i.ID == 1).bids.Add(newBid);
            _context.SaveChanges();
            
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            return RedirectToAction("AuctionList", "Auction");
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBid(BiddingViewModel bvm)
        {

            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == bvm.auctionToSend.ID);
            Bid newBid = new Bid()
            {
                bid = decimal.Parse(bvm.bid.ToString()),
                bidAuthor = user.Email,
                bidDate = DateTime.Now,
                auctionId = tmp.ID

            };
            _context.Bids.Add(newBid);
            _context.SaveChanges();
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            return RedirectToAction("AuctionList", "Auction");
        }



        [Authorize]
        [HttpGet]
        public Auctions GetAuction(int id)
        {
          
            var y = (Auctions)_context.Auctions.Where(d => d.ID == id).ToList()[0];
            return (Auctions) _context.Auctions.Where(d => d.ID == id).ToList()[0];
        }

        public async Task<ActionResult> End(int id)
        {
            Auctions e = _context.Entry<Auctions>(GetAuction(id)).Entity;
            e.endDate = DateTime.Now.ToString();
            e.state = "ended";
            _context.SaveChanges();

            return RedirectToAction("AuctionList", "Auction");
        }
    }

}
