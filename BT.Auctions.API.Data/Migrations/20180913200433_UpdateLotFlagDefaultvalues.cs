using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class UpdateLotFlagDefaultvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSalePriceHidden",
                table: "Lots",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "IsDisplayed",
                table: "Images",
                nullable: false,
                oldClrType: typeof(bool),
                oldDefaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsSalePriceHidden",
                table: "Lots",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDisplayed",
                table: "Images",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool));
        }
    }
}
