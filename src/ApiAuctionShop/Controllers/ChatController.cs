using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Authorization;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNet.Http;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using ImageProcessor.Imaging;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ApiAuctionShop.Models;
using ApiAuctionShop.Helpers;
using ApiAuctionShop.Database;
using System.Text;
using System.Linq;

namespace Projekt.Controllers
{
    /// <summary>
    ///  Nie ma routingu bo nie wiem jak zrobić to + zeby Startup.cs ogarniał 
    ///  ze to jest do string id a nie kolejna ścieżka :)
    /// </summary>
    /// 
    ///dodac authorize naglowki [authorize] 
    public class ChatController : Controller
    {
        public ApplicationDbContext context;

        public ChatController(ApplicationDbContext _context)
        {
            context = _context;
        }
        [HttpGet]
        public IActionResult Get(string id)
        {
           var messagescount = context.chat.Where(d => d.toperson == id).Where(d => d.sendedmsg == true)
                .Select(x => new {
                    someProperty = x.author,
                    someOtherProperty = x.message,
                }).ToList();
            return new JsonResult(messagescount);
        }

    }

}
