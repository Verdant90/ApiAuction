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
    
    public class AccountController : Controller
    {
        private readonly UserManager<Signup> _userManager;
        private readonly SignInManager<Signup> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ApplicationDbContext _context;

        public AccountController(
            UserManager<Signup> userManager,
            SignInManager<Signup> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        //sprawdzanie tokena (unikalny link) 
        [AllowAnonymous]
        public async Task<IActionResult> Urllogin(string id)
        {
            var decryptedstring_encoded = Encoding.Default.GetString(Convert.FromBase64String(id));
            string[] decryptedstring = StringCipher.Decrypt(decryptedstring_encoded, Settings.HashPassword);

            if (!(decryptedstring[0] == ""))
            {
                var result = await _signInManager.PasswordSignInAsync(decryptedstring[0], decryptedstring[0] + "0D?", isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Login", "Account");
        }
        
        //zwroc view logowania
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //wysylanie maila
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Signup model)
        {
            if (ModelState.IsValid)
            {
                string encryptedstring = StringCipher.Encrypt(model.Email, Settings.HashPassword);
                var encrypt2 = Convert.ToBase64String(Encoding.UTF8.GetBytes(encryptedstring));

                await EmailSender.SendEmailAsync(model.Email, "URL do zalogowania", "http://projektgrupowy.azurewebsites.net/Account/Urllogin/" + encrypt2);
                ModelState.AddModelError(string.Empty, "Wysłane");

                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Signup model)
        {
            if (ModelState.IsValid)
            {
                var user = new Signup { UserName = model.Email, Email = model.Email};
                              
                var result = await _userManager.CreateAsync(user, model.Email + "0D?");
                
                //////////////////TEST ROLES//////////////////////////////////////////
                string name = "User";
                bool roleExist = await _roleManager.RoleExistsAsync(name);
                if (!roleExist)
                {
                    var roleresult = _roleManager.CreateAsync(new IdentityRole(name));
                }
                await _userManager.AddToRoleAsync(user, name);
                //////////////////////////////////////////////////////////////////////

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }

        
        

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }

}
