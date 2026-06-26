using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace jwt_rest_api.Migrations
{
    /// <inheritdoc />
    public partial class AddGameSessionsArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastPlayedSessionId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EventDataMaster",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventName = table.Column<string>(type: "text", nullable: false),
                    ArcName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDataMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    SessionDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    SessionStartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionEndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SessionIsFinished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubEventDataMaster",
                columns: table => new
                {
                    EncounterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    EncounterDomain = table.Column<string>(type: "text", nullable: false),
                    EncounterValue = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubEventDataMaster", x => x.EncounterId);
                    table.ForeignKey(
                        name: "FK_SubEventDataMaster_EventDataMaster_EventId",
                        column: x => x.EventId,
                        principalTable: "EventDataMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SessionId = table.Column<string>(type: "text", nullable: false),
                    EventDataId = table.Column<int>(type: "integer", nullable: false),
                    EventDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    EventStartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventEndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionEvents_EventDataMaster_EventDataId",
                        column: x => x.EventDataId,
                        principalTable: "EventDataMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionEvents_GameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventEncounters",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    EncounterObject = table.Column<string>(type: "text", nullable: false),
                    EncounterQuestion = table.Column<string>(type: "text", nullable: false),
                    EncounterDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    EncounterStartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EncounterEndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEncounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventEncounters_SessionEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "SessionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionMinigames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: false),
                    MinigameDurationSeconds = table.Column<int>(type: "integer", nullable: false),
                    MinigameStartDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MinigameEndDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MinigameIsRelatedWithEncounterId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionMinigames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionMinigames_SessionEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "SessionEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EncounterId = table.Column<string>(type: "text", nullable: false),
                    ResponseText = table.Column<string>(type: "text", nullable: false),
                    Domain = table.Column<string>(type: "text", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerResponses_EventEncounters_EncounterId",
                        column: x => x.EncounterId,
                        principalTable: "EventEncounters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventEncounters_EventId",
                table: "EventEncounters",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserId",
                table: "GameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerResponses_EncounterId",
                table: "PlayerResponses",
                column: "EncounterId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionEvents_EventDataId",
                table: "SessionEvents",
                column: "EventDataId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionEvents_SessionId",
                table: "SessionEvents",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionMinigames_EventId",
                table: "SessionMinigames",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SubEventDataMaster_EventId",
                table: "SubEventDataMaster",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerResponses");

            migrationBuilder.DropTable(
                name: "SessionMinigames");

            migrationBuilder.DropTable(
                name: "SubEventDataMaster");

            migrationBuilder.DropTable(
                name: "EventEncounters");

            migrationBuilder.DropTable(
                name: "SessionEvents");

            migrationBuilder.DropTable(
                name: "EventDataMaster");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastPlayedSessionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SessionCount",
                table: "Users");
        }
    }
}
