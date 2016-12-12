using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class BiddingViewModel
    {
        public Auctions auctionToSend { get; set; }
        public bool hasBuyNowGlobal { get; set; }
        public decimal bid { get; set; }
        public List<Bid> bids { get; set; }
    }
    
}
