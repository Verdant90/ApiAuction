using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class ImageFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Auctions")]
        public int AuctionId { get; set; }
        public virtual Auctions Auction { get; set; }
        public string ImagePath { get; set; }
        //public virtual Auctions Auction { get; set; }
    }
}
