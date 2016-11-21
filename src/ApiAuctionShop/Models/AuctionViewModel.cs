using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuctionShop.Models
{
    public class AuctionViewModel
    {
        public int ID { get; set; }
        public string title { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string state { get; set; }
        public int bidCount { get; set; }
        public bool editable { get; set; }
        public decimal highestBid { get; set; }
        public Signup Signup { get; set; }
        public decimal startPrice { get; set; }
        public TimeLeft timeLeft { get; set; }
        public String ImageData { get; set; }
        public Signup winner { get; set; }
        public bool isWatched { get; set; }
    }
    public class TimeLeft
    {
        public TimeLeft(int number, string measure)
        {
            timeMeasure = measure;
            howManyLeft = number;
        }
        public int howManyLeft { get; set; }
        public string timeMeasure { get; set; }
    }
}
