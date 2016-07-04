using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Authorization;
using ApiAuctionShop.Attributes;
using ApiAuctionShop.Database;
using System.IO;
using ImageProcessor;
using ImageProcessor.Imaging;
using System.Drawing;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc.Filters;
using System.Text;
using ApiAuctionShop.Helpers;
using Microsoft.AspNet.Http;

namespace Projekt.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [LoginAuthentication]
    public class MobileauctionController : Controller
    {
        private readonly UserManager<Signup> _userManager;
        private string email;
        public ApplicationDbContext context;
        public MobileauctionController(ApplicationDbContext _context, UserManager<Signup> userManager, IHttpContextAccessor contextAccessor)
        {
            context = _context;
            _userManager = userManager;

            var auth = contextAccessor.HttpContext.Request.Headers["Authorization"];
            int i = auth.ToString().IndexOf("Basic ");
            string code = auth.ToString().Substring(i + "Basic ".Length);
            var token = Encoding.Default.GetString(Convert.FromBase64String(code));
            var encrypt = StringCipher.Decrypt(token, Settings.HashPassword);
            email = encrypt[0];
        }

        [HttpGet]
        public async Task<ObjectResult> Get()
        {

            var user = _userManager.Users.First(d => d.Email == email);

            var list = context.Auctions.Where(d => d.SignupId == user.Id).ToList();

            var listfixed = new List<MobileAuctionSender>();

            foreach (var el in list)
            {
                MobileAuctionSender dynamic = new MobileAuctionSender
                {
                    description = el.description,
                    duration = el.duration,
                    ID = el.ID,
                    ImageData = el.ImageData,
                    ImageMimeType = el.ImageMimeType,
                    price = el.price,
                    title = el.title
                };
                listfixed.Add(dynamic);
            }

            return new HttpOkObjectResult(listfixed);
        }

        [HttpPost]
        public async Task<ObjectResult> Post([FromBody] MobileAuctionSender value)
        {

            Stream stream = new MemoryStream(value.ImageData);
            using (var ms = new MemoryStream())
            {
                using (var imageFactory = new ImageFactory())
                {
                    imageFactory.FixGamma = false;
                    imageFactory.Load(stream).Resize(new ResizeLayer(new Size(400, 400), ResizeMode.Stretch))
                    .Format(new JpegFormat
                    {
                        Quality = 100
                    })
                    .Quality(100)
                    .Save(ms);
                }
                var fileBytes = ms.ToArray();

                var _auction = new Auctions()
                {
                    title = value.title,
                    description = value.description,
                    duration = value.duration,
                    price = value.price,
                    ImageData = fileBytes
                };

                var user = _userManager.Users.First(d => d.Email == email);

                user.Auction.Add(_auction);

                await _userManager.UpdateAsync(user);

                return new HttpOkObjectResult(value);
            }

        }
    }
}