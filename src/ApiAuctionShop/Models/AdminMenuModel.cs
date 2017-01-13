using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class AdminMenuModel
    {
        public int auctionsCount { get; set; }
        public int activeAuctionCount { get; set; }
        public int bidsCount { get; set; }
        public int userCount { get; set; }
        public int auctionsWon { get; set; }

        public AdminMenuModel( int auctionsCount, int activeAuctionCount, int bidsCount,int userCount, int auctionsWon)
        {
            this.auctionsCount = auctionsCount;
            this.activeAuctionCount = activeAuctionCount;
            this.bidsCount = bidsCount;
            this.userCount = userCount;
            this.auctionsWon = auctionsWon;
        }
    }
}
