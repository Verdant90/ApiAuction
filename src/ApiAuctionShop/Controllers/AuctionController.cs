
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Projekt.Controllers
{
    //kontroler dla aukcji 
    public class AuctionController : Controller
    {
        public ApplicationDbContext _context;
        private readonly UserManager<Signup> _userManager;
        private readonly IHostingEnvironment _environment;
        public Dictionary<string, Dictionary<string,string>> dict;
        public AuctionController(IHostingEnvironment environment,
            UserManager<Signup> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _environment = environment;
            string xmlString = System.IO.File.ReadAllText(@"Resources/translations.xml");
            var document = System.Xml.Linq.XDocument.Parse(xmlString);
            var settingsList = (from element in document.Root.Elements("word")
                                select new Setting
                                {
                                    Code = element.Attribute("code").Value,
                                    Lang = element.Attribute("lang").Value,
                                    Value = element.Value
                                }).ToList();
            dict = settingsList.GroupBy(i => i.Code)
                            .ToDictionary(j => j.Key,
                                          k => k.ToDictionary(i => i.Lang, thing => thing.Value));
            
        }


        [AllowAnonymous]
        public ActionResult AuctionPage(int id)
        {
            //var bids = _context.Bids.Count(i => i.auctionId == id);
            var bids = _context.Bids.Where(i => i.auctionId == id).ToList().OrderByDescending(o => o.bid).ToList();
            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == id);
            var author = _context.Users.FirstOrDefault(user => user.Id == tmp.SignupId).Email;
            tmp.author = author;
            var images = _context.ImageFiles.Where(i => i.AuctionId == id).ToList(); // lazy loading: wystarczy się odwołać do ImagesFiles żeby zostały załadowane do aukcji
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            BiddingViewModel bvm = new BiddingViewModel
            {
                auctionToSend = tmp,
                hasBuyNowGlobal = settings.hasBuyNow,
                bids = bids,
                d = dict
            };

            return View(bvm);
        }

        //zmienic nazwe na AuctionLists
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> AuctionList()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var users = _context.Users;
            var bids = _context.Bids;
            var list_mine = _context.Auctions.Where(d => d.SignupId == user.Id).ToList();
            List<List<AuctionViewModel>> model = new List<List<AuctionViewModel>>();
            List<AuctionViewModel> lineMine = new List<AuctionViewModel>();
            model.Add(new List<AuctionViewModel>()); //my auctions
            model.Add(new List<AuctionViewModel>()); //active auctions 
            model.Add(new List<AuctionViewModel>()); //archived auctions 
            foreach (Auctions auction in list_mine)
            {
                AuctionViewModel tmp = new AuctionViewModel() {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    editable = auction.editable,
                    bidCount = bids.Where(b => b.auctionId == auction.ID).ToList().Count(),
                    Signup = users.FirstOrDefault(u => u.Id == auction.SignupId),
                    
                };
                _context.ImageFiles.Where(i => i.AuctionId == auction.ID).ToList(); // lazy loading: wystarczy się odwołać do ImagesFiles żeby zostały załadowane do aukcji
                if (auction.imageFiles != null)
                    tmp.ImageData = auction.imageFiles.ElementAt(0).ImagePath;

                if (bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                tmp.highestBid = bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                model[0].Add(tmp);
            }
            
            var list_active = _context.Auctions.Where(a => a.state == "active").ToList();
            foreach (Auctions auction in list_active)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    bidCount = bids.Where(b => b.auctionId == auction.ID).ToList().Count(),
                    Signup = users.FirstOrDefault(u => u.Id == auction.SignupId)
                };

                if (auction.imageFiles != null)
                    tmp.ImageData = auction.imageFiles.ElementAt(0).ImagePath;

                if (bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                    tmp.highestBid = bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                model[1].Add(tmp);

            }

            var list_ended = _context.Auctions.Where(a => a.state == "ended").ToList();
            foreach (Auctions auction in list_ended)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    bidCount = bids.Where(b => b.auctionId == auction.ID).ToList().Count(),
                    Signup = users.FirstOrDefault(u => u.Id == auction.SignupId),
                    winner = auction.winner
                };

                if (auction.imageFiles != null)
                    tmp.ImageData = auction.imageFiles.ElementAt(0).ImagePath;

                if (bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                    tmp.highestBid = bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                
                model[2].Add(tmp);

            }

            return View(model);
        }

        //////////////////TEST /////////////////////////
        [Authorize]
        public IActionResult AddAuction()
        {
            AuctionCreateViewModel model = new AuctionCreateViewModel();
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            model.hasBuyNowGlobal = settings.hasBuyNow;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddAuction(AuctionCreateViewModel acvm, bool? now, ICollection<IFormFile> files = null)
        {
            TryValidateModel(acvm.auction);
            if(now == null)
                DateValidation(acvm.auction);
            else
                DateValidation(acvm.auction, true);
            PriceValidation(acvm.auction);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                            .Select(x => new { x.Key, x.Value.Errors });
                AuctionCreateViewModel model = new AuctionCreateViewModel();
                var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
                model.hasBuyNowGlobal = settings.hasBuyNow;
                return View(model);
            }
            
            string sqlFormattedDate;
            if (now != null)
            {
                sqlFormattedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }else
            {
                sqlFormattedDate = DateTime.Parse(acvm.auction.startDate).ToString("yyyy-MM-dd HH:mm:ss");
            }
            var _auction = new Auctions()
            {
                title = acvm.auction.title,
                description = acvm.auction.description,
                startDate = sqlFormattedDate,

                endDate = DateTime.Parse(acvm.auction.endDate).ToString("yyyy-MM-dd HH:mm:ss"),
                startPrice = acvm.auction.startPrice,
                buyPrice = acvm.auction.buyPrice,
                author = acvm.auction.author,
                editable = acvm.auction.editable

            //currentPrice = (decimal)auction.price,
            };
            foreach(var file in files)
            { 
                if (file != null)
                {
                    if (file.ContentType.Contains("image"))
                    {
                        using (var fileStream = file.OpenReadStream())
                        {

                            var uploads = Path.Combine(_environment.WebRootPath, "images");
                            Directory.CreateDirectory(uploads);
                            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                            var fullpath = Path.Combine(uploads, fileName);
                            using (var fs = new FileStream(fullpath, FileMode.Create))
                            {

                                await fileStream.CopyToAsync(fs);
                            }

                            var img = new ImageFile()
                            {
                                ImagePath = fullpath,
                                Auction = _auction
                            };
                            _context.ImageFiles.Add(img);
                            
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
            AuctionCreateViewModel acvm = new AuctionCreateViewModel();
            acvm.auction = _context.Auctions.First(i => i.ID == id);
            Auctions auctionToEdit = _context.Auctions.First(i => i.ID == id);
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            acvm.hasBuyNowGlobal = settings.hasBuyNow;
            if (auctionToEdit == null)
            {
                return HttpNotFound();
            }
            return View(acvm);
        }
        
        // POST: /Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]

        public async Task<ActionResult> Edit(AuctionCreateViewModel acvm, bool? now, IFormFile file = null)
        {
            DateValidation(acvm.auction);
            PriceValidation(acvm.auction);
            if (!ModelState.IsValid)
            {
                var errorLog = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
                AuctionCreateViewModel acvm2 = new AuctionCreateViewModel();
                acvm.auction = _context.Auctions.First(i => i.ID == acvm.auction.ID);
                Auctions auctionToEdit = _context.Auctions.First(i => i.ID == acvm.auction.ID);
                var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
                acvm.hasBuyNowGlobal = settings.hasBuyNow;
                return View(acvm);
            }
            else
            { 
                var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());

                
                var tmp = _context.Auctions.FirstOrDefault(i => i.ID == acvm.auction.ID); 
                if (tmp != null)
                {
                    if(tmp.state == "waiting")
                    {
                        tmp.title = acvm.auction.title;
                        tmp.description = acvm.auction.description;
                        tmp.buyPrice = acvm.auction.buyPrice;
                        tmp.endDate = acvm.auction.endDate;
                        tmp.startPrice = acvm.auction.startPrice;
                        tmp.startDate = (now != null)? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : acvm.auction.startDate;
                        tmp.editable = acvm.auction.editable;
                    }else if (tmp.state == "active")
                    {
                        tmp.endDate = acvm.auction.endDate;
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
                                // błąd po dodaniu wielu zdjęć i usunięciu 
                                //tmp.ImageData = fileBytes;
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


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBid(BiddingViewModel bvm)
        {

            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var highestBid = (_context.Bids.Where(b => b.auctionId == bvm.auctionToSend.ID).ToList().Count <= 0)?0:_context.Bids.Where(b => b.auctionId == bvm.auctionToSend.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == bvm.auctionToSend.ID);
            if (highestBid >= bvm.bid || bvm.bid < tmp.startPrice || user.Id == tmp.SignupId) return RedirectToAction("AuctionPage", "Auction", new { id = bvm.auctionToSend.ID });
            if (tmp.buyPrice != null && bvm.bid > tmp.buyPrice) bvm.bid = (decimal) tmp.buyPrice;
            Bid newBid = new Bid()
            {
                bid = bvm.bid,
                bidAuthor = user.Email,
                bidDate = DateTime.Now.ToString(),
                auctionId = tmp.ID

            };
            if(bvm.bid >= tmp.buyPrice)
            {
                //end auction
                tmp.winnerID = user.Id;
                tmp.endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tmp.state = "ended";
            }
            _context.Bids.Add(newBid);
            _context.SaveChanges();
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            return RedirectToAction("AuctionPage", "Auction", new { id = bvm.auctionToSend.ID } );
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyNow(BiddingViewModel bvm)
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var highestBid = (_context.Bids.Where(b => b.auctionId == bvm.auctionToSend.ID).ToList().Count <= 0) ? 0 : _context.Bids.Where(b => b.auctionId == bvm.auctionToSend.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == bvm.auctionToSend.ID);
            if (user.Id == tmp.SignupId) return RedirectToAction("AuctionPage", "Auction", new { id = bvm.auctionToSend.ID });

            if (tmp.state == "active")
            {
                Bid newBid = new Bid()
                {
                    bid = (decimal) tmp.buyPrice,
                    bidAuthor = user.Email,
                    bidDate = DateTime.Now.ToString(),
                    auctionId = tmp.ID

                };
                _context.Bids.Add(newBid);
                tmp.winnerID = user.Id;
                tmp.state = "ended";
                tmp.endDate = DateTime.Now.ToString();
                _context.SaveChanges();
                
            }
            return RedirectToAction("AuctionPage", "Auction", new { id = bvm.auctionToSend.ID });
        }


        [Authorize]
        [HttpGet]
        public Auctions GetAuction(int id)
        {
          
            var y = (Auctions)_context.Auctions.Where(d => d.ID == id).ToList()[0];
            return (Auctions) _context.Auctions.Where(d => d.ID == id).ToList()[0];
        }


        private void EndAuction(int idAuction, int idUser)
        {

        }

        public async Task<ActionResult> End(int id)
        {
            Auctions e = _context.Entry<Auctions>(GetAuction(id)).Entity;
            e.endDate = DateTime.Now.ToString();
            e.state = "ended";
            _context.SaveChanges();

            return RedirectToAction("AuctionList", "Auction");
        }

        private TimeLeft calculateTimeLeft(DateTime d)
        {
            if (d < DateTime.Now) return new TimeLeft(-1, "minut");
            if((d - DateTime.Now).Days > 0)
            {
                if ((d - DateTime.Now).Days == 1) return new TimeLeft((d - DateTime.Now).Days, "dzień");
                else return new TimeLeft((d - DateTime.Now).Days, "dni");
            }
            else if((d - DateTime.Now).Hours > 0)
            {
                if ((d - DateTime.Now).Hours == 1) return new TimeLeft(1, "godzina");
                else if ((d - DateTime.Now).Hours > 1 && (d - DateTime.Now).Hours < 5) return new TimeLeft((d - DateTime.Now).Hours, "godziny");
                else return new TimeLeft((d - DateTime.Now).Hours, "godzin");
            }else return new TimeLeft((d - DateTime.Now).Minutes, "minut");
            
        }
        private void DateValidation(Auctions auction, bool ignoreStartDate = false)
        {
            DateTime startDate, endDate;
            if (!DateTime.TryParse(auction.startDate, out startDate))
            {
                if(!ignoreStartDate)
                    ModelState.AddModelError("startDate", "Wrong start date format!");
            }
            else if (!ignoreStartDate && startDate.CompareTo(DateTime.Now) < 1)
            {

                ModelState.AddModelError("startDate", "Start date must be later than now!");
            }
            if (!DateTime.TryParse(auction.endDate, out endDate))
            {
                ModelState.AddModelError("endDate", "Wrong end date format!");
            }
            else
            {
                if (endDate.CompareTo(DateTime.Now) < 1)
                    ModelState.AddModelError("endDate", "End date must be later than now!");
                if (!ignoreStartDate && endDate.CompareTo(startDate) < 1)
                    ModelState.AddModelError("endDate", "End date must be later than the start date!");
            }
        }
        private void PriceValidation(Auctions auction)
        {
            if (auction.buyPrice <= auction.startPrice)
                ModelState.AddModelError("buyPrice", "Buy price must be greater than the start price!");
        }
    }
    public class Setting
    {
        public string Code { get; set; }
        public string Lang { get; set; }
        public string Value { get; set; }
    }
}