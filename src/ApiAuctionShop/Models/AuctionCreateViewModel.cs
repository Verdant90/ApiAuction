using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Models
{
    public class AuctionCreateViewModel 
    {
        public Auctions auction { get; set; }
        public bool hasBuyNowGlobal { get; set; }
    }
}
