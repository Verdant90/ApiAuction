using ApiAuctionShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNet.Mvc.Localization;
using Projekt.Controllers;
using ApiAuctionShop.Database;
using Microsoft.AspNet.Identity;
using ApiAuctionShop.Models;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;

namespace ApiAuctionShop.Tests
{
    public class HomeControllerTest
    {
        public static Mock<SignInManager<TUser>> MockSignInManager<TUser>() where TUser : class
        {
            var context = new Mock<HttpContext>();
            var manager = MockUserManager<TUser>();
            return new Mock<SignInManager<TUser>>(manager.Object,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<TUser>>().Object,
                null, null)
            { CallBase = true };
        }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            IList<IUserValidator<TUser>> UserValidators = new List<IUserValidator<TUser>>();
            IList<IPasswordValidator<TUser>> PasswordValidators = new List<IPasswordValidator<TUser>>();

            var store = new Mock<IUserStore<TUser>>();
            UserValidators.Add(new UserValidator<TUser>());
            PasswordValidators.Add(new PasswordValidator<TUser>());
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, UserValidators, PasswordValidators, null, null, null, null, null);
            return mgr;
        }

        [Fact]
        public void HomeTest()
        {
            var MockUserManager = MockUserManager<Signup>();
            var mockRepository1 = new Mock<IHtmlLocalizer<HomeController>>();
            var mockRepository2 = new Mock<ApplicationDbContext>();

            var controller = new HomeController(MockUserManager.Object, mockRepository2.Object, mockRepository1.Object);
            var x = controller.Index("en");


        }
    }
}
