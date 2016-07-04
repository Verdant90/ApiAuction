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
using System.Text;
using ApiAuctionShop.Database;
using ApiAuctionShop.Helpers;

namespace Projekt.Controllers
{
    public class MobileaccountController : Controller
    {
        public ApplicationDbContext context;
        public MobileaccountController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpPost]
        [Route("api/[controller]")]
        [Produces("application/json")]
        public ObjectResult Post([FromBody] Signup value)
        {
            var encrypt = StringCipher.Encrypt(value.Email, Settings.HashPassword);
            var encrypt2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(encrypt));

            var myEmails = context.Logins.Where(s => s.Email == value.Email);

            if (myEmails.Any())
            {
                myEmails.First().IsTokenConfirmed = false;
                context.SaveChanges();
                EmailSender.SendEmailAsync(value.Email, "Token", "http://projektgrupowy.azurewebsites.net/mobileaccount/get/" + encrypt2);
            }
            return new HttpOkObjectResult(encrypt2);
        }

        [HttpGet]
        public string Get(string id)
        {
            var encrypt2 = Encoding.Default.GetString(Convert.FromBase64String(id));
            string[] encrypt = StringCipher.Decrypt(encrypt2, Settings.HashPassword);

            var user = context.Logins.Where(s => s.Email == encrypt[0]);

            user.First().IsTokenConfirmed = true;
            user.First().ExpireTokenTime = encrypt[1];
            user.First().Token = id;

            context.SaveChanges();

            return "Token added " + encrypt[0];
        }
    }
}






/**
else
{
    value.Email = value.Email;
    value.IsTokenConfirmed = false;
    value.ExpireTokenTime = null;
    value.Id = context.Logins.Max(record => record.Id) + 1;
    context.Logins.Add(value);
    context.SaveChanges();
    EmailSender.SendEmailAsync(value.Email, "Token", "http://projektgrupowy.azurewebsites.net/api/mobileaccount/" + encrypt2);

}**/
