using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class Bid
    {
        public string bidAuthor { get; set; }
        public decimal bid { get; set; }
        public DateTime bidDate { get; set; }
    }
}
