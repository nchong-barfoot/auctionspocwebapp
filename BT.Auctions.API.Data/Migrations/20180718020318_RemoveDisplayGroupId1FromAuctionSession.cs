using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class RemoveDisplayGroupId1FromAuctionSession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId1",
                table: "AuctionSessions");

            migrationBuilder.DropIndex(
                name: "IX_AuctionSessions_DisplayGroupId",
                table: "AuctionSessions");

            migrationBuilder.DropIndex(
                name: "IX_AuctionSessions_DisplayGroupId1",
                table: "AuctionSessions");

            migrationBuilder.DropColumn(
                name: "DisplayGroupId1",
                table: "AuctionSessions");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionSessions_DisplayGroupId",
                table: "AuctionSessions",
                column: "DisplayGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuctionSessions_DisplayGroupId",
                table: "AuctionSessions");

            migrationBuilder.AddColumn<int>(
                name: "DisplayGroupId1",
                table: "AuctionSessions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuctionSessions_DisplayGroupId",
                table: "AuctionSessions",
                column: "DisplayGroupId",
                unique: true,
                filter: "[DisplayGroupId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionSessions_DisplayGroupId1",
                table: "AuctionSessions",
                column: "DisplayGroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId1",
                table: "AuctionSessions",
                column: "DisplayGroupId1",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
