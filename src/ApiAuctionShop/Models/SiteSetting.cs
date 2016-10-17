using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class SiteSetting
    {
        [Key]
        public int id { get; set; }
        public bool hasBuyNow { get; set; }
        public string timePeriods { get; set; }
        public string colorTheme { get; set; }
        public string photoSize { get; set; }
        public string startMessage { get; set; }

    }
}
