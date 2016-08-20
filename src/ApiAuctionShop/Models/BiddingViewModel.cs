using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class BiddingViewModel
    {
        public Auctions auctionToSend { get; set; }
        public double bid { get; set; }
    }
    
}
