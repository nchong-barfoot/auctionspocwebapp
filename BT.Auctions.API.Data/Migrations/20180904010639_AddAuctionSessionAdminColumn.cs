using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class AddAuctionSessionAdminColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuctionSessionAdmin",
                table: "AuctionSessions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuctionSessionAdmin",
                table: "AuctionSessions");
        }
    }
}
