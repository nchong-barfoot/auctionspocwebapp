using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class LotInfoRenameAndIndexUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotAgent");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.CreateTable(
                name: "LotDetails",
                columns: table => new
                {
                    LotDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LotId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotDetails", x => x.LotDetailId);
                    table.ForeignKey(
                        name: "FK_LotDetails_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotDetails_LotId",
                table: "LotDetails",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_LotDetails_Key_LotId",
                table: "LotDetails",
                columns: new[] { "Key", "LotId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotDetails");

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    AgentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgentIdentifier = table.Column<string>(nullable: false),
                    EmailAddress = table.Column<string>(nullable: false),
                    FullName = table.Column<string>(nullable: false),
                    ProfileImageUrl = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.AgentId);
                });

            migrationBuilder.CreateTable(
                name: "LotAgent",
                columns: table => new
                {
                    AgentId = table.Column<int>(nullable: false),
                    LotId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotAgent", x => new { x.AgentId, x.LotId });
                    table.ForeignKey(
                        name: "FK_LotAgent_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "AgentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LotAgent_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_AgentIdentifier",
                table: "Agents",
                column: "AgentIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LotAgent_LotId",
                table: "LotAgent",
                column: "LotId");
        }
    }
}
