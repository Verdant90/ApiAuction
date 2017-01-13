using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Models
{
    public class AdminStatisticsViewModel 
    {
        public AdminMenuModel adminMenuModel { get; set; }
        public List<Auctions> auctions;
        public List<Bid> bids;
        public List<Signup> users;
        public int lastWeekAuctionsPercent { get; set; }
        public int[] lastWeekAuctionsCount;
        public int[] auctionStates;
        public int[] currentMonthBids;
        public AdminStatisticsViewModel()
        {
            lastWeekAuctionsCount = new int[7];
            auctionStates = new int[4];
            currentMonthBids = new int[31];
            auctions = new List<Auctions>();
            bids = new List<Bid>();
            users = new List<Signup>();
            adminMenuModel = new AdminMenuModel(0, 0, 0, 0, 0);
        }
    }
}
