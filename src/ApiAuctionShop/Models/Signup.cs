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

        //zmiana na date
        public int duration { get; set; }

        //w perspektywie: wiecej zdjec
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }

        public string description { get; set; }

        //zmienic na decimal(2)
        public int price { get; set; }

        public string title { get; set; }

        //public string authorEmail { get; set; }
        //public string auctionState { get; set; } 
        /// <summary>
        /// states: pending, ended, cancelled
        /// </summary>
        //public string cathegory { get; set; }
        public Signup Signup { get; set; }

        //id aukcji (rzeczywiste) 
        [Column("SignupId")]
        public string SignupId { get; set; }
    }
}