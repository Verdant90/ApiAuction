using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using ApiAuctionShop.Models;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using ApiAuctionShop.Database;
using Microsoft.AspNet.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {

        public ApplicationDbContext _context;
        private readonly UserManager<Signup> _userManager;
        

        public AdminPanelController(
            UserManager<Signup> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;

        }

        // GET: /AdminPanel/
        public IActionResult AdminPanel()
        {
            var users = _context.Users;
            var userCount = users.Count();
            var bidsCount = _context.Bids.Count();
            var auctionsCount = _context.Auctions.Count();
            var activeAuctionsCount = _context.Auctions.Where(a => a.state == "active").Count();
            var auctionsWon = _context.Auctions.Where(a => a.state == "ended" && a.winner != null).Count();
            AdminMenuModel model = new AdminMenuModel(auctionsCount, activeAuctionsCount, bidsCount, userCount, auctionsWon);
            
            return View("Index", model);
        }

        public AdminMenuModel GetAdminMenuModel()
        {
            var users = _context.Users.ToList();
            var userCount = users.Count();
            var bidsCount = _context.Bids.Count();
            var auctionsCount = _context.Auctions.Count();
            var activeAuctionsCount = _context.Auctions.Where(a => a.state == "active").Count();
            var auctionsWon = _context.Auctions.Where(a => a.state == "ended" && a.winner != null).Count();
            return new AdminMenuModel(auctionsCount, activeAuctionsCount, bidsCount, userCount, auctionsWon);

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Auctions()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var users = _context.Users;
            var list_auctions = _context.Auctions.ToList();
            AdminAuctionsViewModel model = new AdminAuctionsViewModel();

            foreach (Auctions auction in list_auctions)
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
                    bidCount = _context.Bids.Where(b => b.auctionId == auction.ID).ToList().Count(),
                    Signup = users.FirstOrDefault(u => u.Id == auction.SignupId)
                };

                if (_context.Bids.Where(b => b.auctionId == auction.ID).ToList().Count > 0)
                    tmp.highestBid = _context.Bids.Where(b => b.auctionId == auction.ID).ToList().OrderByDescending(i => i.bid).ToList().FirstOrDefault().bid;
                model.auctions.Add(tmp);
            }
            var userCount = users.Count();
            var bidsCount = _context.Bids.Count();
            var auctionsCount = _context.Auctions.Count();
            var activeAuctionsCount = _context.Auctions.Where(a => a.state == "active").Count();
            var auctionsWon = _context.Auctions.Where(a => a.state == "ended" && a.winner != null).Count();
            model.adminMenuModel.userCount = userCount;
            model.adminMenuModel.bidsCount = bidsCount;
            model.adminMenuModel.auctionsCount = auctionsCount;
            model.adminMenuModel.activeAuctionCount = activeAuctionsCount;
            model.adminMenuModel.auctionsWon = auctionsWon;

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Statistics()
        {
            string[] states = {"active","waiting","ended","inactive" };
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var users = _context.Users.ToList();
            var list_auctions = _context.Auctions.ToList();
            var bids = _context.Bids.ToList();
            AdminStatisticsViewModel model = new AdminStatisticsViewModel();
            model.auctions = list_auctions;
            model.bids = bids;
            model.users = users;
           
            model.adminMenuModel = GetAdminMenuModel();
            //last week auctions
            for(int i = -7; i < 0; ++i)
            {
                model.lastWeekAuctionsCount[7 + i] = model.auctions.Where(a => DateTime.Parse(a.startDate).ToString("yyyy-MM-dd") == DateTime.Today.AddDays(i).ToString("yyyy-MM-dd")).Count(); 
            }
            //all auction states
            for (int i = 0; i < 4; ++i)
            {
                //0-active,1-waiting,2-ended,3-inactive
                model.auctionStates[i] = model.auctions.Where(a => a.state == states[i]).Count();
            }
            for (int i = 0; i < 31; ++i)
            {
                //0-active,1-waiting,2-ended,3-inactive
                model.currentMonthBids[i] = model.bids.Where(b => DateTime.Parse(b.bidDate).Month == DateTime.Today.Month && DateTime.Parse(b.bidDate).Year == DateTime.Today.Year && DateTime.Parse(b.bidDate).Day == i+1).Count();
            }

            model.lastWeekAuctionsPercent = (model.lastWeekAuctionsCount.Sum() *100) / model.auctions.Count();
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Users()
        {
            var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
            var users = _context.Users.ToList();
            AdminUsersModel model = new AdminUsersModel();
            foreach (Signup signup in users)
            {
                SignupViewModel tmp = new SignupViewModel()
                {
                    email = signup.Email,
                    registeredDate = "-",
                    auctionsCount = _context.Auctions.Where(d => d.SignupId == signup.Id).Count(),
                    bidsCount = _context.Bids.Where(d => d.bidAuthor == signup.Email).Count(),
                    auctionsWonCount = _context.Auctions.Where(d => d.winnerID == signup.Id).Count(),
                    soldItemsCount = _context.Auctions.Where(d => d.SignupId == signup.Id && d.state == "ended" && d.winnerID != null).Count()
                };
                model.users.Add(tmp);
            }
           
            model.adminMenuModel = GetAdminMenuModel();

            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Settings()
        {
            var users = _context.Users.ToList();
            var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
            AdminSettingsViewModel model = new AdminSettingsViewModel();
            model.timePeriods = settings.timePeriods;
            model.hasBuyNow = settings.hasBuyNow;
            model.photoSize = settings.photoSize;
            model.startMessage = settings.startMessage;
            model.adminMenuModel = GetAdminMenuModel();
            return View(model);

        }


        [Authorize]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Settings(AdminSettingsViewModel asvm)
        {
            if (ModelState.IsValid)
            {
                var users = _context.Users.ToList();
                var settings = _context.Settings.Where(setting => setting.id == 1).FirstOrDefault();
                settings.hasBuyNow = asvm.hasBuyNow;
                settings.timePeriods = asvm.timePeriods;
                settings.startMessage = asvm.startMessage;
                _context.SaveChanges();
                
            }
            asvm.adminMenuModel = GetAdminMenuModel();
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
            return View(asvm);
        }
    }
}
