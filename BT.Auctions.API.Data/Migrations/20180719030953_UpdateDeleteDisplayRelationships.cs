using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class UpdateDeleteDisplayRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration",
                column: "DisplayConfigurationId",
                principalTable: "DisplayConfigurations",
                principalColumn: "DisplayConfigurationId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration",
                column: "DisplayConfigurationId",
                principalTable: "DisplayConfigurations",
                principalColumn: "DisplayConfigurationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
