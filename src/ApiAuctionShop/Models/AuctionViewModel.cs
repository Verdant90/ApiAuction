using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class AuctionViewModel
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string state { get; set; }
        public int bidCount { get; set; }
        public decimal highestBid { get; set; }
        public Signup Signup { get; set; }
        public decimal startPrice { get; set; }

    }
}
