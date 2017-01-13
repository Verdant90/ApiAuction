using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace ApiAuctionShop.Migrations
{
    public partial class testmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_Signup_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_Signup_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_Signup_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "duration", table: "Auctions");
            migrationBuilder.CreateTable(
                name: "Bid",
                columns: table => new
                {
                    bidId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    auctionId = table.Column<int>(nullable: false),
                    bid = table.Column<decimal>(nullable: false),
                    bidAuthor = table.Column<string>(nullable: true),
                    bidDate = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bid", x => x.bidId);
                    table.ForeignKey(
                        name: "FK_Bid_Auctions_auctionId",
                        column: x => x.auctionId,
                        principalTable: "Auctions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "SiteSetting",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColorTheme = table.Column<string>(nullable: true),
                    hasBuyNow = table.Column<bool>(nullable: false),
                    photoSize = table.Column<int>(nullable: false),
                    timePeriods = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSetting", x => x.id);
                });
            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "Auctions",
                nullable: false);
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Auctions",
                nullable: false);
            migrationBuilder.AddColumn<string>(
                name: "author",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "bid",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddColumn<decimal>(
                name: "buyPrice",
                table: "Auctions",
                nullable: false,
                defaultValue: 0m);
            migrationBuilder.AddColumn<string>(
                name: "cathegory",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddColumn<bool>(
                name: "editable",
                table: "Auctions",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "endDate",
                table: "Auctions",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<string>(
                name: "startDate",
                table: "Auctions",
                nullable: false,
                defaultValue: "");
            migrationBuilder.AddColumn<decimal>(
                name: "startPrice",
                table: "Auctions",
                nullable: false,
                defaultValue: 0m);
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "winnerID",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Signup_winnerID",
                table: "Auctions",
                column: "winnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_Signup_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_Signup_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_Signup_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Auctions_Signup_winnerID", table: "Auctions");
            migrationBuilder.DropForeignKey(name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId", table: "AspNetRoleClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserClaim<string>_Signup_UserId", table: "AspNetUserClaims");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserLogin<string>_Signup_UserId", table: "AspNetUserLogins");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_IdentityRole_RoleId", table: "AspNetUserRoles");
            migrationBuilder.DropForeignKey(name: "FK_IdentityUserRole<string>_Signup_UserId", table: "AspNetUserRoles");
            migrationBuilder.DropColumn(name: "author", table: "Auctions");
            migrationBuilder.DropColumn(name: "bid", table: "Auctions");
            migrationBuilder.DropColumn(name: "buyPrice", table: "Auctions");
            migrationBuilder.DropColumn(name: "cathegory", table: "Auctions");
            migrationBuilder.DropColumn(name: "editable", table: "Auctions");
            migrationBuilder.DropColumn(name: "endDate", table: "Auctions");
            migrationBuilder.DropColumn(name: "startDate", table: "Auctions");
            migrationBuilder.DropColumn(name: "startPrice", table: "Auctions");
            migrationBuilder.DropColumn(name: "state", table: "Auctions");
            migrationBuilder.DropColumn(name: "winnerID", table: "Auctions");
            migrationBuilder.DropTable("Bid");
            migrationBuilder.DropTable("SiteSetting");
            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "Auctions",
                nullable: true);
            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "Auctions",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserClaim<string>_Signup_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserLogin<string>_Signup_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
            migrationBuilder.AddForeignKey(
                name: "FK_IdentityUserRole<string>_Signup_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
