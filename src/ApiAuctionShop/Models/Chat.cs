using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAuctionShop.Models
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string author { get; set; }

        public DateTime messagedate { get; set; }

        public string message { get; set; }

        public string toperson { get; set; }

        public bool sendedmsg { get; set; }
    }
}