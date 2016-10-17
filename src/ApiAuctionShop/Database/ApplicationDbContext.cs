using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Projekt.Controllers;
using ApiAuctionShop.Models;

namespace ApiAuctionShop.Database
{
    public class ApplicationDbContext : IdentityDbContext<Signup>
    {
        public DbSet<Signup> Logins { get; set; }
        public DbSet<SiteSetting> Settings { get; set; }
        public DbSet<Auctions> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auctions>()
              .HasOne(p => p.Signup)
              .WithMany(b => b.Auction);

            modelBuilder.Entity<Auctions>()
                .HasOne(p => p.winner)
                .WithMany(w => w.AuctionsWon);

            modelBuilder.Entity<Bid>()
              .HasOne(p => p.Auction)
              .WithMany(b => b.bids);

            base.OnModelCreating(modelBuilder);
        }
    }
}
