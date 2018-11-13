using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BT.Auctions.API.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "DisplayGroups",
                columns: table => new
                {
                    DisplayGroupId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayGroups", x => x.DisplayGroupId);
                });

            migrationBuilder.CreateTable(
                name: "Venues",
                columns: table => new
                {
                    VenueId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    Suburb = table.Column<string>(nullable: false),
                    Region = table.Column<string>(nullable: false),
                    OnSite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Venues", x => x.VenueId);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuctionSessions",
                columns: table => new
                {
                    AuctionSessionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VenueId = table.Column<int>(nullable: true),
                    DisplayGroupId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    FinishDate = table.Column<DateTime>(nullable: true),
                    DisplayGroupId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionSessions", x => x.AuctionSessionId);
                    table.ForeignKey(
                        name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId",
                        column: x => x.DisplayGroupId,
                        principalTable: "DisplayGroups",
                        principalColumn: "DisplayGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionSessions_DisplayGroups_DisplayGroupId1",
                        column: x => x.DisplayGroupId1,
                        principalTable: "DisplayGroups",
                        principalColumn: "DisplayGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuctionSessions_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Displays",
                columns: table => new
                {
                    DisplayId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VenueId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Displays", x => x.DisplayId);
                    table.ForeignKey(
                        name: "FK_Displays_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    LotId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuctionSessionId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    ReserveMet = table.Column<bool>(nullable: false),
                    ListingId = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: false),
                    PostponedDateTime = table.Column<DateTime>(nullable: true),
                    PlusGST = table.Column<bool>(nullable: false),
                    AuctionStatus = table.Column<string>(nullable: false),
                    FeatureDescription = table.Column<string>(nullable: true),
                    SoldDate = table.Column<DateTime>(nullable: true),
                    SoldPrice = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.LotId);
                    table.ForeignKey(
                        name: "FK_Lots_AuctionSessions_AuctionSessionId",
                        column: x => x.AuctionSessionId,
                        principalTable: "AuctionSessions",
                        principalColumn: "AuctionSessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DisplayConfigurations",
                columns: table => new
                {
                    DisplayConfigurationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DisplayId = table.Column<int>(nullable: false),
                    VenueId = table.Column<int>(nullable: false),
                    PlayVideo = table.Column<bool>(nullable: false),
                    DisplayMode = table.Column<string>(nullable: false),
                    PlayAudio = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayConfigurations", x => x.DisplayConfigurationId);
                    table.ForeignKey(
                        name: "FK_DisplayConfigurations_Displays_DisplayId",
                        column: x => x.DisplayId,
                        principalTable: "Displays",
                        principalColumn: "DisplayId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisplayConfigurations_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "VenueId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LotId = table.Column<int>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Images_Lots_LotId",
                        column: x => x.LotId,
                        principalTable: "Lots",
                        principalColumn: "LotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LotAgent",
                columns: table => new
                {
                    LotId = table.Column<int>(nullable: false),
                    AgentId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DisplayGroupConfiguration",
                columns: table => new
                {
                    DisplayGroupId = table.Column<int>(nullable: false),
                    DisplayConfigurationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisplayGroupConfiguration", x => new { x.DisplayConfigurationId, x.DisplayGroupId });
                    table.ForeignKey(
                        name: "FK_DisplayGroupConfiguration_DisplayConfigurations_DisplayConfigurationId",
                        column: x => x.DisplayConfigurationId,
                        principalTable: "DisplayConfigurations",
                        principalColumn: "DisplayConfigurationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DisplayGroupConfiguration_DisplayGroups_DisplayGroupId",
                        column: x => x.DisplayGroupId,
                        principalTable: "DisplayGroups",
                        principalColumn: "DisplayGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agents_AgentIdentifier",
                table: "Agents",
                column: "AgentIdentifier",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AuctionSessions_VenueId",
                table: "AuctionSessions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayConfigurations_DisplayId",
                table: "DisplayConfigurations",
                column: "DisplayId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayConfigurations_VenueId",
                table: "DisplayConfigurations",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_DisplayGroupConfiguration_DisplayGroupId",
                table: "DisplayGroupConfiguration",
                column: "DisplayGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Displays_VenueId",
                table: "Displays",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_LotId",
                table: "Images",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_LotAgent_LotId",
                table: "LotAgent",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_AuctionSessionId",
                table: "Lots",
                column: "AuctionSessionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisplayGroupConfiguration");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "LotAgent");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "DisplayConfigurations");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "Displays");

            migrationBuilder.DropTable(
                name: "AuctionSessions");

            migrationBuilder.DropTable(
                name: "DisplayGroups");

            migrationBuilder.DropTable(
                name: "Venues");
        }
    }
}
