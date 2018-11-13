using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class MoveVenueIdToDisplayGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayConfigurations_Venues_VenueId",
                table: "DisplayConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_DisplayConfigurations_VenueId",
                table: "DisplayConfigurations");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "DisplayConfigurations");

            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                table: "DisplayGroups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DisplayGroups_VenueId",
                table: "DisplayGroups",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroups_Venues_VenueId",
                table: "DisplayGroups",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroups_Venues_VenueId",
                table: "DisplayGroups");

            migrationBuilder.DropIndex(
                name: "IX_DisplayGroups_VenueId",
                table: "DisplayGroups");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "DisplayGroups");

            migrationBuilder.AddColumn<int>(
                name: "VenueId",
                table: "DisplayConfigurations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DisplayConfigurations_VenueId",
                table: "DisplayConfigurations",
                column: "VenueId");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayConfigurations_Venues_VenueId",
                table: "DisplayConfigurations",
                column: "VenueId",
                principalTable: "Venues",
                principalColumn: "VenueId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
