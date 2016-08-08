using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class MyViewModel
    {
        public string bid { get; set; }
        public Auctions auction { get; set; }
        public MyViewModel(Auctions auction)
        {
            this.auction = auction;
            this.bid = "";
        }
        public MyViewModel()
        {
            this.auction = new Auctions();
            this.bid = "";
        }
    }
    
}
