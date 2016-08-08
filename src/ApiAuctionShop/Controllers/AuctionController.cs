﻿
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
            //var viewModel = new MyViewModel(GetAuction(id));

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
                            DateTime myDateTime = DateTime.Now;
                            string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                            var _auction = new Auctions()
                            {
                                title = auction.title,
                                description = auction.description,
                                startDate = sqlFormattedDate,
                                endDate = DateTime.Parse( auction.endDate).ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            //currentPrice = (decimal)auction.price,
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
        public async Task<ActionResult> Edit(Auctions auction)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());


                SqlConnection sqlConnection1 = new SqlConnection("server=SZYMON\\SQLEXPRESS;database=master;Integrated Security=SSPI;");
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "UPDATE dbo.Auctions SET title = '"+ auction.title +"', description = '"+auction.description+"', endDate = '"+auction.endDate+"' where id = " + auction.ID;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                // Data is accessible through the DataReader object here.

                sqlConnection1.Close();

                var tmp = _context.Auctions.FirstOrDefault(i => i.ID == auction.ID);
                if(tmp != null)
                {
                    tmp.title = auction.title;

                }
                //FIX ME

            }
            return RedirectToAction("AuctionList", "Auction");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddBid(Auctions au)
        {
            //auction's Id is not set here :/
                Bid newBid = new Bid();
                var user = await _userManager.FindByIdAsync(HttpContext.User.GetUserId());
                newBid.bid = decimal.Parse(au.bid);
                newBid.bidAuthor = user.Email;
                newBid.bidDate = DateTime.Now;
                List<Bid> auctionBids = au.bids;
                auctionBids.Add(newBid);
                _context.Auctions.FirstOrDefault(i => i.ID == 1).bids.Add(newBid);


                SqlConnection sqlConnection1 = new SqlConnection("server=SZYMON\\SQLEXPRESS;database=master;Integrated Security=SSPI;");
                SqlCommand cmd = new SqlCommand();
                SqlDataReader reader;

                cmd.CommandText = "UPDATE dbo.Auctions SET bids = CAST('" + auctionBids +  "' AS varbinary) where id = 1";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();

                reader = cmd.ExecuteReader();
                // Data is accessible through the DataReader object here.

                sqlConnection1.Close();

            
            var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
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
