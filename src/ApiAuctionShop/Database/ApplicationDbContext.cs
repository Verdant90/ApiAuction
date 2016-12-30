using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Projekt.Controllers;
using ApiAuctionShop.Models;
using Microsoft.Data.Entity.Infrastructure;

namespace ApiAuctionShop.Database
{
    public class ApplicationDbContext : IdentityDbContext<Signup>
    {
        public DbSet<Signup> Logins { get; set; }
        public DbSet<SiteSetting> Settings { get; set; }
        public DbSet<Auctions> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<ImageFile> ImageFiles { get; set; }
        public DbSet<AuctionsUsersWatching> AuctionsUsersWatching { get; set; }
        public DbSet<Chat> chat { get; set; }
        public ApplicationDbContext()
        : base()
        {
            
        }
        
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


            modelBuilder.Entity<ImageFile>()
                .HasOne(p => p.Auction)
                .WithMany(i => i.imageFiles);


            modelBuilder.Entity<AuctionsUsersWatching>()
            .HasKey(t => new { t.AuctionId, t.UserId });

            modelBuilder.Entity<AuctionsUsersWatching>()
                .HasOne(pt => pt.Auction)
                .WithMany(p => p.AuctionsUsersWatching)
                .HasForeignKey(pt => pt.AuctionId);

            modelBuilder.Entity<AuctionsUsersWatching>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.AuctionsUsersWatching)
                .HasForeignKey(pt => pt.UserId);

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\v11.0;Initial Catalog=ProjektGrupowy;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
}
