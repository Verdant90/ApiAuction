using ApiAuctionShop.Database;
using ApiAuctionShop.Helpers;
using ApiAuctionShop.Models;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AuctionPreview(AuctionCreateViewModel acvm, ICollection<IFormFile> files = null)
        {
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            AuctionCreateViewModel tmp = new AuctionCreateViewModel()
            {
                auction = acvm.auction,
                hasBuyNowGlobal = settings.hasBuyNow,
                timePeriods = settings.timePeriods
            };
            tmp.auction.imageFiles = new List<ImageFile>();
            foreach (var file in files)
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
                                Auction = tmp.auction
                            };
                            tmp.auction.imageFiles.Add(img);

                        }
                    }
                }
            }

            return View(tmp);
        }

        //zmienic nazwe na AuctionLists
        [AllowAnonymous]
        [HttpGet]
        public IActionResult AuctionList()
        {
            return View();
        }

        //////////////////TEST /////////////////////////
        [Authorize]
        public IActionResult AddAuction()
        {
            AuctionCreateViewModel model = new AuctionCreateViewModel();
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            model.hasBuyNowGlobal = settings.hasBuyNow;
            model.timePeriods = settings.timePeriods;
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddAuction(AuctionCreateViewModel acvm, string submit, ICollection<ImageFile> files)
        {
            if (submit.Equals("cancel"))
            {
                return View("AddAuction", acvm);
            }

            TryValidateModel(acvm.auction);

            PriceValidation(acvm.auction);

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                            .Select(x => new { x.Key, x.Value.Errors });
                AuctionCreateViewModel model = new AuctionCreateViewModel();
                var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
                model.hasBuyNowGlobal = settings.hasBuyNow;
                model.timePeriods = settings.timePeriods;
                return View(model);
            }
            DateTime dtnow = DateTime.Now;
            DateTime dtend = calculateEndDate(dtnow, acvm.auction.duration);

            string sqlFormattedStartDate = dtnow.ToString("yyyy-MM-dd HH:mm:ss");
            string sqlFormattedEndDate = dtend.ToString("yyyy-MM-dd HH:mm:ss");
            var _auction = new Auctions()
            {
                title = acvm.auction.title,
                description = acvm.auction.description,
                startDate = sqlFormattedStartDate,
                endDate = sqlFormattedEndDate,
                startPrice = acvm.auction.startPrice,
                buyPrice = acvm.auction.buyPrice,
                author = acvm.auction.author,
                editable = acvm.auction.editable

            //currentPrice = (decimal)auction.price,
            };
            foreach(var file in acvm.auction.imageFiles)
            {
                var img = new ImageFile()
                {
                    ImagePath = file.ImagePath,
                    Auction = _auction
                };
                _context.ImageFiles.Add(img);
            }
            //foreach(var file in files)
            //{ 
            //    if (file != null)
            //    {
            //        if (file.ContentType.Contains("image"))
            //        {
            //            using (var fileStream = file.OpenReadStream())
            //            {

            //                var uploads = Path.Combine(_environment.WebRootPath, "images");
            //                Directory.CreateDirectory(uploads);
            //                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //                var fullpath = Path.Combine(uploads, fileName);
            //                using (var fs = new FileStream(fullpath, FileMode.Create))
            //                {

            //                    await fileStream.CopyToAsync(fs);
            //                }

            //                var img = new ImageFile()
            //                {
            //                    ImagePath = fullpath,
            //                    Auction = _auction
            //                };
            //                _context.ImageFiles.Add(img);
                            
            //            }
            //        }
            //    }
            //}
            
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
            acvm.timePeriods = settings.timePeriods;
            _context.ImageFiles.Where(i => i.AuctionId == acvm.auction.ID).ToList(); 
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

        public async Task<ActionResult> Edit(AuctionCreateViewModel acvm, IFormFile file = null)
        {
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
                acvm.timePeriods = settings.timePeriods;
                _context.ImageFiles.Where(i => i.AuctionId == acvm.auction.ID).ToList();
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
                        tmp.startDate = acvm.auction.startDate;
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
        public async Task<ActionResult> AddImage(AuctionCreateViewModel acvm, IFormFile file = null)
        {

            var tmp = _context.Auctions.FirstOrDefault(i => i.ID == acvm.auction.ID);
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
                            Auction = tmp
                        };
                        _context.ImageFiles.Add(img);
                        _context.SaveChanges();
                    }
                }
            }
            return RedirectToAction("AuctionList", "Auction");
        }
        public async Task<ActionResult> EditImage(int id, IFormFile file = null)
        {
            var imageToChange = _context.ImageFiles.SingleOrDefault(i => i.ID == id);
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

                        imageToChange.ImagePath = fullpath;
                        _context.SaveChanges();


                    }
                }
            }
            return RedirectToAction("AuctionList", "Auction");
        }
        public async Task<ActionResult> DeleteImage(int id)
        {
            var imageToRemove = _context.ImageFiles.SingleOrDefault(i => i.ID == id);
            _context.ImageFiles.Remove(imageToRemove);
            _context.SaveChanges();
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
            notifyWatchers(bvm.auctionToSend.ID);
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
                notifyWatchers(tmp.ID);
                
            }
            return RedirectToAction("AuctionPage", "Auction", new { id = bvm.auctionToSend.ID });
        }


        public async void notifyWatchers(int auctionId)
        {
            string title ="", message = "" ; 
            var usersWatching = _context.AuctionsUsersWatching.Where(a => a.AuctionId == auctionId).ToList();
            
            foreach(AuctionsUsersWatching auw in usersWatching)
            {
                if(_context.Users.Where(u => u.Id == auw.UserId).FirstOrDefault().Id == _context.Auctions.Where(a => auw.AuctionId == auctionId).FirstOrDefault().SignupId)
                {
                    title = "Nowa oferta kupna w Twojej aukcji!";
                    message = "Witaj! Ktoś zalicytował w Twojej aukcji: <br/> http://localhost:5000/pl-PL/Auction/AuctionPage/";
                }else
                {
                    title = "Nowa oferta kupna w aukcji, którą obserwujesz";
                    message = "Witaj! Ktoś złożył ofertę w aukcji, którą obserwujesz: <br/> http://localhost:5000/pl-PL/Auction/AuctionPage/";
                }
                await EmailSender.SendEmailAsync(_context.Users.Where(u=> u.Id == auw.UserId).FirstOrDefault().Email, title, message + auw.AuctionId);

            }

            
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
        private DateTime calculateEndDate(DateTime d, string duration)
        {
            if(duration.Contains("h"))
            {
                int hours = int.Parse(duration.Replace("h", ""));
                d = d.AddHours(hours);
            }
            else if(duration.Contains("d"))
            {
                int days = int.Parse(duration.Replace("d", ""));
                d = d.AddDays(days);
            }
            else if(duration.Contains("w"))
            {
                int weeks = int.Parse(duration.Replace("w", ""));
                d = d.AddDays(7 * weeks);
            }
            return d;
        }
        private void PriceValidation(Auctions auction)
        {
            if (auction.buyPrice <= auction.startPrice)
                ModelState.AddModelError("auction.buyPrice", "Buy price must be greater than the start price!");
        }
    }
    public class Setting
    {
        public string Code { get; set; }
        public string Lang { get; set; }
        public string Value { get; set; }
    }
}