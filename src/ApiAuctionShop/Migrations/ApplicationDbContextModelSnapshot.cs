using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using ApiAuctionShop.Database;

namespace ApiAuctionShop.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ApiAuctionShop.Models.Auctions", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SignupId")
                        .HasAnnotation("Relational:ColumnName", "SignupId");

                    b.Property<string>("author");

                    b.Property<string>("bid");

                    b.Property<decimal?>("buyPrice");

                    b.Property<string>("cathegory");

                    b.Property<string>("description")
                        .IsRequired();

                    b.Property<string>("duration");

                    b.Property<bool>("editable");

                    b.Property<string>("endDate");

                    b.Property<int>("price");

                    b.Property<string>("startDate");

                    b.Property<decimal>("startPrice");

                    b.Property<string>("state");

                    b.Property<string>("title")
                        .IsRequired();

                    b.Property<string>("winnerID");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.AuctionsUsersWatching", b =>
                {
                    b.Property<int>("AuctionId");

                    b.Property<string>("UserId");

                    b.HasKey("AuctionId", "UserId");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.Bid", b =>
                {
                    b.Property<int>("bidId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("auctionId");

                    b.Property<decimal>("bid");

                    b.Property<string>("bidAuthor");

                    b.Property<string>("bidDate");

                    b.HasKey("bidId");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.Chat", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("author");

                    b.Property<string>("message");

                    b.Property<DateTime>("messagedate");

                    b.Property<bool>("sendedmsg");

                    b.Property<string>("toperson");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.ImageFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AuctionId");

                    b.Property<string>("ImagePath");

                    b.HasKey("ID");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.Signup", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("ExpireTokenTime");

                    b.Property<bool>("IsTokenConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Token");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasAnnotation("Relational:Name", "EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .HasAnnotation("Relational:Name", "UserNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetUsers");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.SiteSetting", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("colorTheme");

                    b.Property<bool>("hasBuyNow");

                    b.Property<string>("photoSize");

                    b.Property<string>("startMessage");

                    b.Property<string>("timePeriods");

                    b.HasKey("id");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasAnnotation("Relational:Name", "RoleNameIndex");

                    b.HasAnnotation("Relational:TableName", "AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasAnnotation("Relational:TableName", "AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasAnnotation("Relational:TableName", "AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasAnnotation("Relational:TableName", "AspNetUserRoles");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.Auctions", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("SignupId");

                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("winnerID");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.AuctionsUsersWatching", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Auctions")
                        .WithMany()
                        .HasForeignKey("AuctionId");

                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.Bid", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Auctions")
                        .WithMany()
                        .HasForeignKey("auctionId");
                });

            modelBuilder.Entity("ApiAuctionShop.Models.ImageFile", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Auctions")
                        .WithMany()
                        .HasForeignKey("AuctionId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNet.Identity.EntityFramework.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId");

                    b.HasOne("ApiAuctionShop.Models.Signup")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
        }
    }
}
