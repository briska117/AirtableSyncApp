using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AirTableDatabase.Migrations
{
    /// <inheritdoc />
    public partial class AirtablesApiDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientPrefixes",
                columns: table => new
                {
                    ClientPrefixId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPrefixes", x => x.ClientPrefixId);
                });

            migrationBuilder.CreateTable(
                name: "CollectionModeRelatedTables",
                columns: table => new
                {
                    CollectionModeRelatedTableId = table.Column<string>(type: "text", nullable: false),
                    CollectionModeId = table.Column<string>(type: "text", nullable: false),
                    RelatedTableId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionModeRelatedTables", x => x.CollectionModeRelatedTableId);
                });

            migrationBuilder.CreateTable(
                name: "CollectionModes",
                columns: table => new
                {
                    CollectionModeId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionModes", x => x.CollectionModeId);
                });

            migrationBuilder.CreateTable(
                name: "CountryPrefixes",
                columns: table => new
                {
                    CountryPrefixId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CountryPrefixes", x => x.CountryPrefixId);
                });

            migrationBuilder.CreateTable(
                name: "RelatedTables",
                columns: table => new
                {
                    RelatedTableId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TableId = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatedTables", x => x.RelatedTableId);
                });

            migrationBuilder.CreateTable(
                name: "SyncEvents",
                columns: table => new
                {
                    SyncEventId = table.Column<string>(type: "text", nullable: false),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<string>(type: "text", nullable: false),
                    SyncTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncEvents", x => x.SyncEventId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    ApiKey = table.Column<string>(type: "text", nullable: false),
                    ClientPrefixId = table.Column<string>(type: "text", nullable: false),
                    Mode = table.Column<string>(type: "text", nullable: false),
                    CountryPrefixId = table.Column<string>(type: "text", nullable: false),
                    MainDatabaseID = table.Column<string>(type: "text", nullable: false),
                    TableSheetsToken = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Projects_ClientPrefixes_ClientPrefixId",
                        column: x => x.ClientPrefixId,
                        principalTable: "ClientPrefixes",
                        principalColumn: "ClientPrefixId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_CollectionModes_Mode",
                        column: x => x.Mode,
                        principalTable: "CollectionModes",
                        principalColumn: "CollectionModeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_CountryPrefixes_CountryPrefixId",
                        column: x => x.CountryPrefixId,
                        principalTable: "CountryPrefixes",
                        principalColumn: "CountryPrefixId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventHistories",
                columns: table => new
                {
                    SyncEventHistoryId = table.Column<string>(type: "text", nullable: false),
                    SyncEventId = table.Column<string>(type: "text", nullable: false),
                    StartAsync = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishAsync = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    StatusAsync = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHistories", x => x.SyncEventHistoryId);
                    table.ForeignKey(
                        name: "FK_EventHistories_SyncEvents_SyncEventId",
                        column: x => x.SyncEventId,
                        principalTable: "SyncEvents",
                        principalColumn: "SyncEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProjects",
                columns: table => new
                {
                    UserProjectId = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjects", x => x.UserProjectId);
                    table.ForeignKey(
                        name: "FK_UserProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventHistories_SyncEventId",
                table: "EventHistories",
                column: "SyncEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ClientPrefixId",
                table: "Projects",
                column: "ClientPrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CountryPrefixId",
                table: "Projects",
                column: "CountryPrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Mode",
                table: "Projects",
                column: "Mode");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjects_ProjectId",
                table: "UserProjects",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionModeRelatedTables");

            migrationBuilder.DropTable(
                name: "EventHistories");

            migrationBuilder.DropTable(
                name: "RelatedTables");

            migrationBuilder.DropTable(
                name: "UserProjects");

            migrationBuilder.DropTable(
                name: "SyncEvents");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ClientPrefixes");

            migrationBuilder.DropTable(
                name: "CollectionModes");

            migrationBuilder.DropTable(
                name: "CountryPrefixes");
        }
    }
}
