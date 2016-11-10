using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAuctionShop.Models
{
    public class AdminUsersModel 
    {
        public AdminMenuModel adminMenuModel;
        public List<SignupViewModel> users;
        public AdminUsersModel()
        {
            users = new List<SignupViewModel>();
            adminMenuModel = new AdminMenuModel(0, 0, 0, 0, 0);
        }
    }
}
