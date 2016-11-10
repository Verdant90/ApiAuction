using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Models
{
    public class AdminAuctionsViewModel 
    {
        public AdminMenuModel adminMenuModel;
        public List<AuctionViewModel> auctions;
        public AdminAuctionsViewModel()
        {
            auctions = new List<AuctionViewModel>();
            adminMenuModel = new AdminMenuModel(0,0,0,0,0);
        }
    }
}
