using System.Linq;
using Microsoft.AspNet.Mvc;
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using Newtonsoft.Json;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Routing;
using System.Text.RegularExpressions;
using Microsoft.Data.Entity;

namespace ApiAuctionShop.Controllers
{
    // API sluzace do zapytan AJAXowych odnosnie wyswietlania listy aukcji na AuctionList
    [Produces("application/json")]
    [Route("api/APIAuctions")]
    public class APIAuctionViewModelController : Controller
    {
        private ApplicationDbContext _context;

        public APIAuctionViewModelController(ApplicationDbContext context)
        {
            _context = context;
        }
        // pobranie wszystkich aukcji
        [HttpGet]
        public Object GetAuctions(int start = 0, int length = 10, int draw = 0)
        {
            //Access to everybody
            // if (!User.IsSignedIn())   
            //     return new HttpStatusCodeResult(StatusCodes.Status403Forbidden);

            var orderby = Request.Query["order[0][column]"];
            var order = Request.Query["order[0][dir]"];
            var searchString = Request.Query["search[value]"].ToString().ToLower();

            var userId = User.GetUserId();
            var users = _context.Users;
            IQueryable<Auctions> query;
            query = _context.Auctions.Where(a => a.state == "active").Include(a => a.AuctionsUsersWatching).Include(a => a.bids);
            var totalCount = query.Count();
            var recordsFiltered = totalCount;
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.title.ToLower().Contains(searchString) || s.endDate.Contains(searchString));
                recordsFiltered = query.Count();
            }
            switch (orderby)
            {
                case "0":   //title
                    query = (order == "asc") ? query.OrderBy(c => c.title).AsQueryable() : query.OrderByDescending(c => c.title).AsQueryable();
                    break;

                case "1":
                    //query = (order == "asc") ? query.OrderBy(a => a.currentPrice).AsQueryable() : query.OrderByDescending(a => a.currentPrice).AsQueryable();
                    break;

                case "2":   //date
                    query = (order == "asc") ? query.OrderBy(c => c.endDate).AsQueryable() : query.OrderByDescending(c => c.endDate).AsQueryable();
                    break;

                case "3":       //bid count
                    //query = (order == "asc") ? query.OrderBy(c => c.bids.Count()).AsQueryable() : query.OrderByDescending(c => c.bids.Count()).AsQueryable();
                    var selected = query.Select(n => new { N = n, NumberOfBids = n.bids.Count() });//.OrderBy(c => c.Count).Select(n => n.N).AsQueryable();
                    
                    break;
                case "4":      //isWatched
                    query = (order == "asc") ? query.OrderBy(c => c.AuctionsUsersWatching.Where(item => item.UserId == userId && item.AuctionId == c.ID).Count() == 1).AsQueryable() : 
                        query.OrderByDescending(c => c.AuctionsUsersWatching.Where(item => item.UserId == userId && item.AuctionId == c.ID).Count() == 1).AsQueryable();
                    break;
            }
            query = query
               .Skip(start)
               .Take(length);


            List<AuctionViewModel> list_mine = new List<AuctionViewModel>();
            foreach (Auctions auction in query)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    editable = auction.editable,
                    bidCount = auction.bids.Where(b => b.auctionId == auction.ID).Count(),
                    url = Url.Action("AuctionPage", "Auction", new { id = auction.ID }),
                    SignupEmail = users.FirstOrDefault(u => u.Id == auction.SignupId).Email,
                    isWatched = (auction.AuctionsUsersWatching.Where(item => item.UserId == userId && item.AuctionId == auction.ID).Count() == 1)
                };
                _context.ImageFiles.Where(i => i.AuctionId == auction.ID).ToList(); // lazy loading: wystarczy siê odwo³aæ do ImagesFiles ¿eby zosta³y za³adowane do aukcji
                if (auction.imageFiles != null)
                {
                    string path = auction.imageFiles.ElementAt(0).ImagePath;
                    int index = path.IndexOf(@"\images");
                    tmp.ImageData = path.Substring(index);
                }
                else
                {
                    tmp.ImageData = @Url.Content("~/images/noimage.png");
                }

                if (auction.bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                {
                    tmp.highestBid = auction.bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                    tmp.currentPrice = tmp.highestBid;
                }
                else
                {
                    tmp.currentPrice = tmp.startPrice;
                }
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                list_mine.Add(tmp);
            }
            
            return new
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = recordsFiltered,
                data = list_mine
            };
        }


        // pobranie "moich" aukcji
        [HttpGet("mine")]
        public Object GetMyAuctions(int start = 0, int length = 10, int draw = 0)
        {
            if (!User.IsSignedIn())
                return new HttpStatusCodeResult(StatusCodes.Status403Forbidden);

            var orderby = Request.Query["order[0][column]"];
            var order = Request.Query["order[0][dir]"];
            var searchString = Request.Query["search[value]"].ToString().ToLower();

            var users = _context.Users;
            var userId = User.GetUserId();
            IQueryable<Auctions> query;
            query = _context.Auctions.Where(a => a.SignupId == userId).Include(a => a.bids);
            var totalCount = query.Count();
            var recordsFiltered = totalCount;
            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.title.ToLower().Contains(searchString) || s.endDate.Contains(searchString));
                recordsFiltered = query.Count();
            }

            switch (orderby)
            {
                case "0":   //title
                    query = (order == "asc") ? query.OrderBy(c => c.title) : query.OrderByDescending(c => c.title);
                    break;

                case "1":
                    //query = (order == "asc") ? query.OrderBy(a => a.currentPrice) : query.OrderByDescending(a => a.currentPrice);
                    break;

                case "2":   //date
                    query = (order == "asc") ? query.OrderBy(c => c.endDate) : query.OrderByDescending(c => c.endDate);
                    break;

                case "3":       //bid count
                    query = (order == "asc") ? query.OrderBy(c => c.bids.Count()) : query.OrderByDescending(c => c.bids.Count());
                    break;

                case "4":      //state
                    query = (order == "asc") ? query.OrderBy(c => c.state) : query.OrderByDescending(c => c.state);
                    break;
            }

            query = query
                .Skip(start)
                .Take(length);

            List<AuctionViewModel> list_mine = new List<AuctionViewModel>();
            foreach (Auctions auction in query)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    editable = auction.editable,
                    bidCount = auction.bids.Where(b => b.auctionId == auction.ID).Count(),
                    url = Url.Action("AuctionPage", "Auction", new { id = auction.ID }),
                    SignupEmail = users.FirstOrDefault(u => u.Id == auction.SignupId).Email
                };
                _context.ImageFiles.Where(i => i.AuctionId == auction.ID).ToList(); // lazy loading: wystarczy siê odwo³aæ do ImagesFiles ¿eby zosta³y za³adowane do aukcji
                if (auction.imageFiles != null)
                {
                    string path = auction.imageFiles.ElementAt(0).ImagePath;
                    int index = path.IndexOf(@"\images");
                    tmp.ImageData = path.Substring(index);
                }
                else
                {
                    tmp.ImageData = @Url.Content("~/images/noimage.png");
                }

                if (auction.bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                {
                    tmp.highestBid = auction.bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                    tmp.currentPrice = tmp.highestBid;
                }
                else
                {
                    tmp.currentPrice = tmp.startPrice;
                }
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                list_mine.Add(tmp);
            }
            
            return new
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = recordsFiltered,
                data = list_mine
            };
        }


        // aukcje pobrane
        [HttpGet("mine/watched")]
        public Object GetWatchedAuctionsObject(int start = 0, int length = 10, int draw = 0)
        {
            if (!User.IsSignedIn())
                return new HttpStatusCodeResult(StatusCodes.Status403Forbidden);

            var orderby = Request.Query["order[0][column]"];
            var order = Request.Query["order[0][dir]"];
            var searchString = Request.Query["search[value]"].ToString().ToLower();

            var users = _context.Users;
            var bids = _context.Bids;
            var userId = User.GetUserId();
            var auctionsUsersWatching = _context.AuctionsUsersWatching.Where(item => item.UserId == userId);
            IQueryable<Auctions> query;
            query = _context.Auctions.Where(a => auctionsUsersWatching.Any(auw => auw.AuctionId == a.ID));
            //auctionsUsersWatching.
            List<AuctionViewModel> list_mine = new List<AuctionViewModel>();
            foreach (Auctions auction in query)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    editable = auction.editable,
                    bidCount = bids.Where(b => b.auctionId == auction.ID).ToList().Count(),
                    url = Url.Action("AuctionPage", "Auction", new { id = auction.ID }),
                    SignupEmail = users.FirstOrDefault(u => u.Id == auction.SignupId).Email,
                    isWatched = (auctionsUsersWatching.Where(item => item.UserId == userId && item.AuctionId == auction.ID).Count() == 1)
                };
                _context.ImageFiles.Where(i => i.AuctionId == auction.ID).ToList(); // lazy loading: wystarczy siê odwo³aæ do ImagesFiles ¿eby zosta³y za³adowane do aukcji
                if (auction.imageFiles != null)
                {
                    string path = auction.imageFiles.ElementAt(0).ImagePath;
                    int index = path.IndexOf(@"\images");
                    tmp.ImageData = path.Substring(index);
                }
                else
                {
                    tmp.ImageData = @Url.Content("~/images/noimage.png");
                }

                if (bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                {
                    tmp.highestBid = bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                    tmp.currentPrice = tmp.highestBid;
                }
                else
                {
                    tmp.currentPrice = tmp.startPrice;
                }
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                list_mine.Add(tmp);
            }

            switch (orderby)
            {
                case "0":   //title
                    list_mine = (order == "asc") ? list_mine.OrderBy(c => c.title).ToList() : list_mine.OrderByDescending(c => c.title).ToList();
                    break;

                case "1":
                    list_mine = (order == "asc") ? list_mine.OrderBy(a => a.currentPrice).ToList() : list_mine.OrderByDescending(a => a.currentPrice).ToList();
                    break;

                case "2":   //date
                    list_mine = (order == "asc") ? list_mine.OrderBy(c => c.endDate).ToList() : list_mine.OrderByDescending(c => c.endDate).ToList();
                    break;

                case "3":       //bid count
                    list_mine = (order == "asc") ? list_mine.OrderBy(c => c.bidCount).ToList() : list_mine.OrderByDescending(c => c.bidCount).ToList();
                    break;

                case "4":      //state
                    list_mine = (order == "asc") ? list_mine.OrderBy(c => c.state).ToList() : list_mine.OrderByDescending(c => c.state).ToList();
                    break;
            }


            var totalCount = list_mine.Count();
            var recordsFiltered = totalCount;
            if (!String.IsNullOrEmpty(searchString))
            {
                list_mine = list_mine.Where(s => s.currentPrice.ToString().Contains(searchString)
                               || s.title.ToLower().Contains(searchString) || s.endDate.Contains(searchString)
                               || s.SignupEmail.ToLower().Contains(searchString)).ToList();
                recordsFiltered = list_mine.Count();
            }

            var results = list_mine
                .Skip(start)
                .Take(length)
                .ToList();
            return new
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = recordsFiltered,
                data = results
            };
        }

        // zarchiwizowane aukcje
        [HttpGet("ended")]
        public Object GetEndedAuctions(int start = 0, int length = 10, int draw = 0)
        {
            //NOWE

            //Access to everybody
            // if (!User.IsSignedIn())   
            //     return new HttpStatusCodeResult(StatusCodes.Status403Forbidden);

            var orderby = Request.Query["order[0][column]"];
            var order = Request.Query["order[0][dir]"];
            var searchString = Request.Query["search[value]"].ToString().ToLower();

            var userId = User.GetUserId();
            var users = _context.Users;


            IQueryable<Auctions> query;
            query = _context.Auctions.Where(a => a.state == "ended").Include(a => a.bids).Include(a => a.winner);

            var totalCount = query.Count();
            var recordsFiltered = totalCount;

            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.title.ToLower().Contains(searchString) || s.endDate.Contains(searchString));
                recordsFiltered = query.Count();
            }
            switch (orderby)
            {
                case "0":   //title
                    query = (order == "asc") ? query.OrderBy(c => c.title).AsQueryable() : query.OrderByDescending(c => c.title).AsQueryable();
                    break;

                case "1":
                    //query = (order == "asc") ? query.OrderBy(a => a.currentPrice).AsQueryable() : query.OrderByDescending(a => a.currentPrice).AsQueryable();
                    break;

                case "2":   //date
                    query = (order == "asc") ? query.OrderBy(c => c.endDate).AsQueryable() : query.OrderByDescending(c => c.endDate).AsQueryable();
                    break;

                case "3":       //bid count
                    //query = (order == "asc") ? query.OrderBy(c => c.bids.Count()).AsQueryable() : query.OrderByDescending(c => c.bids.Count()).AsQueryable();
                    var selected = query.Select(n => new { N = n, NumberOfBids = n.bids.Count() });//.OrderBy(c => c.Count).Select(n => n.N).AsQueryable();

                    break;
                case "4":      //winnerEmail
                    if(order == "asc")
                    {

                        //union not implemented for iQueryable in mvc core - has to convert to list first -.-
                        var First = query.Where(u => u.winner != null).OrderBy(u=>u.winner.Email).ToList();
                        var Second = query.Where(u => u.winner == null).ToList();
                        query = First.Union(Second).AsQueryable();

                    }else
                    {
                        var First = query.Where(u => u.winner != null).OrderBy(u => u.winner.Email).ToList();
                        var Second = query.Where(u => u.winner == null).ToList();
                        query = Second.Union(First).AsQueryable();
                    }

                    //query = (order == "asc") ? query.OrderBy(o => o.winner == null).ThenBy(o => o.winner.Email) : query.OrderBy(o => o.winner == null).ThenBy(o => o.winner.Email);
                    break;
            }
            query = query
               .Skip(start)
               .Take(length);

            List<AuctionViewModel> list_mine = new List<AuctionViewModel>();
            foreach (Auctions auction in query)
            {
                AuctionViewModel tmp = new AuctionViewModel()
                {
                    ID = auction.ID,
                    title = auction.title,
                    startDate = auction.startDate,
                    endDate = auction.endDate,
                    state = auction.state,
                    startPrice = auction.startPrice,
                    editable = auction.editable,
                    bidCount = auction.bids.Where(b => b.auctionId == auction.ID).Count(),
                    url = Url.Action("AuctionPage", "Auction", new { id = auction.ID }),
                    SignupEmail = users.FirstOrDefault(u => u.Id == auction.SignupId).Email,
                    winnerEmail = (auction.winnerID == null) ? "" : users.FirstOrDefault(u => u.Id == auction.winnerID).Email
                };
                _context.ImageFiles.Where(i => i.AuctionId == auction.ID).ToList(); // lazy loading: wystarczy siê odwo³aæ do ImagesFiles ¿eby zosta³y za³adowane do aukcji
                if (auction.imageFiles != null)
                {
                    string path = auction.imageFiles.ElementAt(0).ImagePath;
                    int index = path.IndexOf(@"\images");
                    tmp.ImageData = path.Substring(index);
                }
                else
                {
                    tmp.ImageData = @Url.Content("~/images/noimage.png");
                }

                if (auction.bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                {
                    tmp.highestBid = auction.bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                    tmp.currentPrice = tmp.highestBid;
                }
                else
                {
                    tmp.currentPrice = tmp.startPrice;
                }
                tmp.timeLeft = calculateTimeLeft(DateTime.Parse(auction.endDate));
                list_mine.Add(tmp);
            }

            return new
            {
                draw = draw,
                recordsTotal = totalCount,
                recordsFiltered = recordsFiltered,
                data = list_mine
            };
        }

        // zwraca czas do konca aukcji
        private TimeLeft calculateTimeLeft(DateTime d)
        {
            if (d < DateTime.Now) return new TimeLeft(-1, "minut");
            if ((d - DateTime.Now).Days > 0)
            {
                if ((d - DateTime.Now).Days == 1) return new TimeLeft((d - DateTime.Now).Days, "dzieñ");
                else return new TimeLeft((d - DateTime.Now).Days, "dni");
            }
            else if ((d - DateTime.Now).Hours > 0)
            {
                if ((d - DateTime.Now).Hours == 1) return new TimeLeft(1, "godzina");
                else if ((d - DateTime.Now).Hours > 1 && (d - DateTime.Now).Hours < 5) return new TimeLeft((d - DateTime.Now).Hours, "godziny");
                else return new TimeLeft((d - DateTime.Now).Hours, "godzin");
            }
            else return new TimeLeft((d - DateTime.Now).Minutes, "minut");

        }
    }

}