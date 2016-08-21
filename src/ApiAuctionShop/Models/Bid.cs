using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class Bid
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bidId { get; set; }

        public int auctionId { get; set; }
        public virtual Auctions Auction { get; set; }
        public string bidAuthor { get; set; }
        public decimal bid { get; set; }
        public string bidDate { get; set; }
    }
}
