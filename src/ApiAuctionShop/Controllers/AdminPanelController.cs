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

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Controllers
{
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
            return View("Index");
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
            model.adminMenuModel.userCount = Convert.ToInt32(userCount);
            model.adminMenuModel.bidsCount = bidsCount;
            model.adminMenuModel.auctionsCount = auctionsCount;
            model.adminMenuModel.activeAuctionCount = activeAuctionsCount;
            model.adminMenuModel.auctionsWon = auctionsWon;

            return View(model);
        }

    }
}
