using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class UpdateDisplayGroupsDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId",
                table: "AuctionSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId",
                table: "AuctionSessions",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId",
                table: "AuctionSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.AddForeignKey(
                name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId",
                table: "AuctionSessions",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
