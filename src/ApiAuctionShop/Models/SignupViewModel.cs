using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class SignupViewModel
    {
        public string email { get; set; }
        public string registeredDate { get; set; }
        public string role { get; set; }
        public int auctionsCount { get; set; }
        public int bidsCount { get; set; }
        public int auctionsWonCount { get; set; }
        public int soldItemsCount { get; set; }
    }
}
