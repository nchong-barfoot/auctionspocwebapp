using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class UpdateDisplayConfigurationAllowDisplayDisassociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisplayGroupConfiguration",
                table: "DisplayGroupConfiguration");

            migrationBuilder.RenameTable(
                name: "DisplayGroupConfiguration",
                newName: "DisplayGroupConfigurations");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayGroupConfiguration_DisplayGroupId",
                table: "DisplayGroupConfigurations",
                newName: "IX_DisplayGroupConfigurations_DisplayGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisplayGroupConfigurations",
                table: "DisplayGroupConfigurations",
                columns: new[] { "DisplayConfigurationId", "DisplayGroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfigurations_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfigurations",
                column: "DisplayConfigurationId",
                principalTable: "DisplayConfigurations",
                principalColumn: "DisplayConfigurationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfigurations_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfigurations",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfigurations_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfigurations");

            migrationBuilder.DropForeignKey(
                name: "FK_DisplayGroupConfigurations_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfigurations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DisplayGroupConfigurations",
                table: "DisplayGroupConfigurations");

            migrationBuilder.RenameTable(
                name: "DisplayGroupConfigurations",
                newName: "DisplayGroupConfiguration");

            migrationBuilder.RenameIndex(
                name: "IX_DisplayGroupConfigurations_DisplayGroupId",
                table: "DisplayGroupConfiguration",
                newName: "IX_DisplayGroupConfiguration_DisplayGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DisplayGroupConfiguration",
                table: "DisplayGroupConfiguration",
                columns: new[] { "DisplayConfigurationId", "DisplayGroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                table: "DisplayGroupConfiguration",
                column: "DisplayConfigurationId",
                principalTable: "DisplayConfigurations",
                principalColumn: "DisplayConfigurationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                table: "DisplayGroupConfiguration",
                column: "DisplayGroupId",
                principalTable: "DisplayGroups",
                principalColumn: "DisplayGroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
