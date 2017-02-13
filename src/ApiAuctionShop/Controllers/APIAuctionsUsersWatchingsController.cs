using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using ApiAuctionShop.Database;
using ApiAuctionShop.Models;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using System;

namespace ApiAuctionShop.Controllers
{
    // API do zapytan AJAXowych dotyczacych obserwowania aukcji poprzez uzytkownikow

    [Produces("application/json")]
    [Route("api/APIAuctionsUsersWatchings")]
    public class APIAuctionsUsersWatchingsController : Controller
    {
        private ApplicationDbContext _context;

        public APIAuctionsUsersWatchingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/APIAuctionsUsersWatchings
        [HttpGet]
        public IEnumerable<AuctionsUsersWatching> GetAuctionsUsersWatching()
        {
            return _context.AuctionsUsersWatching;
        }

        // GET: api/APIAuctionsUsersWatchings/5
        [HttpGet("{id}", Name = "GetAuctionsUsersWatching")]
        public IActionResult GetAuctionsUsersWatching([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            AuctionsUsersWatching auctionsUsersWatching = _context.AuctionsUsersWatching.Single(m => m.AuctionId == id);

            if (auctionsUsersWatching == null)
            {
                return HttpNotFound();
            }

            return Ok(auctionsUsersWatching);
        }

        // PUT: api/APIAuctionsUsersWatchings/5
        [HttpPut("{id}")]
        public IActionResult PutAuctionsUsersWatching(int id, [FromBody] AuctionsUsersWatching auctionsUsersWatching)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            if (id != auctionsUsersWatching.AuctionId)
            {
                return HttpBadRequest();
            }

            _context.Entry(auctionsUsersWatching).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionsUsersWatchingExists(id))
                {
                    return HttpNotFound();
                }
                else
                {
                    throw;
                }
            }

            return new HttpStatusCodeResult(StatusCodes.Status204NoContent);
        }

        // POST: api/APIAuctionsUsersWatchings
        [HttpPost]
        public IActionResult PostAuctionsUsersWatching([FromBody] AuctionsUsersWatching auctionsUsersWatching)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            auctionsUsersWatching.UserId = User.GetUserId();

            if (!(_context.AuctionsUsersWatching.Where(a => a.AuctionId == auctionsUsersWatching.AuctionId && a.UserId == auctionsUsersWatching.UserId).Count() > 0))

                if (User.IsSignedIn())
                {
                    _context.AuctionsUsersWatching.Add(auctionsUsersWatching);
                }
                else
                    return new HttpStatusCodeResult(StatusCodes.Status401Unauthorized);
            else
            {
                return new HttpStatusCodeResult(StatusCodes.Status409Conflict);
            }
            try
            {

                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return new HttpStatusCodeResult(StatusCodes.Status201Created);
        }

        // DELETE: api/APIAuctionsUsersWatchings/5
        [HttpDelete("{id}")]
        public IActionResult DeleteAuctionsUsersWatching(int id)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }
            if (!User.IsSignedIn())
                return new HttpStatusCodeResult(StatusCodes.Status401Unauthorized);

            AuctionsUsersWatching auctionsUsersWatching = _context.AuctionsUsersWatching.Single(m => m.AuctionId == id && m.UserId == User.GetUserId());
            if (auctionsUsersWatching == null)
            {
                return HttpNotFound();
            }

            _context.AuctionsUsersWatching.Remove(auctionsUsersWatching);
            _context.SaveChanges();

            return Ok(auctionsUsersWatching);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuctionsUsersWatchingExists(int id)
        {
            return _context.AuctionsUsersWatching.Count(e => e.AuctionId == id) > 0;
        }
    }
}