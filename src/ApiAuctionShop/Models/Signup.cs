
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class Signup : IdentityUser
    {
        public override string Email { get; set; }
        public string ExpireTokenTime { get; set; }
        public bool IsTokenConfirmed { get; set; }
        public string Token { get; set; }
        public override string UserName { get; set; }
        //musi być zadeklarowana bo inaczej Auction traktuje jak zwykły object = null
        //auctions of a user
        public ICollection<Auctions> Auction { get; set; } = new List<Auctions>();

        public virtual ICollection<Auctions> AuctionsWon { get; set; }

        public List<AuctionsUsersWatching> AuctionsUsersWatching { get; set; } = new List<AuctionsUsersWatching>();


    }

    public class Auctions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string author { get; set; }
        public string winnerID { get; set; }
        public virtual Signup winner { get; set; }

        [Required(ErrorMessage = "The description can't be empty!")]
        public string description { get; set; }

        //zmienic na decimal(2)
        public int price { get; set; }

        [Required(ErrorMessage = "Please enter the title for this auction.")]
        public string title { get; set; }

        public Signup Signup { get; set; }

        //id usera..
        [Column("SignupId")]
        public string SignupId { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Wrong price format!")]
        [Range(1, int.MaxValue, ErrorMessage = "The start price must be greater than 0!")]
        public decimal startPrice { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Wrong price format!")]
        public decimal? buyPrice { get; set; }
        public string state { get; set; } = "waiting";

        [Required(ErrorMessage = "Start date is required!")]
        [DataType(DataType.DateTime, ErrorMessage = "Wrong data format!")]
        public string startDate { get; set; }

        [Required(ErrorMessage = "End date is required!")]
        [DataType(DataType.DateTime, ErrorMessage = "Wrong data format!")]
        public string endDate { get; set; }

        public bool editable { get; set; } = true;

        public string cathegory { get; set; }

        public string bid { get; set; } = "";

        public virtual ICollection<Bid> bids { get; set; }

        public virtual ICollection<ImageFile> imageFiles { get; set; }

        public List<AuctionsUsersWatching> AuctionsUsersWatching { get; set; }
    }

    public class AuctionsUsersWatching
    {
        public int AuctionId { get; set; }
        public Auctions Auction { get; set; }

        public string UserId { get; set; }
        public Signup User { get; set; }
    }
}