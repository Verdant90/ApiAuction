using ApiAuctionShop.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ApiAuctionShop.Tests
{
   
    public class EmailSend
    {
        [Fact]
        public void EmailSendTestFail()
        {
            Assert.Equal(false, EmailSender.SendEmailAsync("test@wp.pl", "test", "test").IsFaulted);
        }
    
    }
}
