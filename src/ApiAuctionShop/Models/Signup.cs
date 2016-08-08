using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAuctionShop.Models
{
    public class Signup : IdentityUser
    {
        public override string Email { get; set; }
        public string ExpireTokenTime { get; set; }
        public bool IsTokenConfirmed { get; set; }
        public string Token { get; set; }
        //musi być zadeklarowana bo inaczej Auction traktuje jak zwykły object = null
        public ICollection<Auctions> Auction { get; set; } = new List<Auctions>(); 
    }

    public class Auctions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        

        //w perspektywie: wiecej zdjec
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public string description { get; set; }

        //zmienic na decimal(2)
        public int price { get; set; }
        public string title { get; set; }
        public Signup Signup { get; set; }

        //id aukcji (rzeczywiste) 
        [Column("SignupId")]
        public string SignupId { get; set; }
        public decimal startPrice { get; set; }
        public decimal buyPrice { get; set; }
        public string state { get; set; } = "active";
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string cathegory { get; set; }
        public string bid { get; set; } = "";
        public List<Bid> bids = new List<Bid>();
    }
}