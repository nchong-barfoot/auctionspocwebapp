using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class AddIndexToAuctionSessionDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AuctionSessions_StartDate_FinishDate",
                table: "AuctionSessions",
                columns: new[] { "StartDate", "FinishDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuctionSessions_StartDate_FinishDate",
                table: "AuctionSessions");
        }
    }
}
