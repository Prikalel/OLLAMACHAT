using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VelikiyPrikalel.OLLAMACHAT.Infrastructure.Context.Migrations
{
    /// <inheritdoc />
    public partial class UnityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserChats_UserChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserChatId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserChatId",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "EventHandlers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ParameterTypes = table.Column<string>(type: "TEXT", nullable: false),
                    ArgumentsHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsInherited = table.Column<bool>(type: "INTEGER", nullable: false),
                    BaseClassFullName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsPublic = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHandlers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnityEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GenericTypeArguments = table.Column<string>(type: "TEXT", nullable: true),
                    ArgumentsHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsInherited = table.Column<bool>(type: "INTEGER", nullable: false),
                    BaseClassFullName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnityEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventHandlerRelationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    EventId = table.Column<string>(type: "TEXT", nullable: false),
                    HandlerId = table.Column<string>(type: "TEXT", nullable: false),
                    EventHandlerRelationshipType = table.Column<int>(type: "INTEGER", nullable: false),
                    Source = table.Column<int>(type: "INTEGER", nullable: false),
                    DeletionReason = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHandlerRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventHandlerRelationships_EventHandlers_HandlerId",
                        column: x => x.HandlerId,
                        principalTable: "EventHandlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventHandlerRelationships_UnityEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "UnityEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_EventId",
                table: "EventHandlerRelationships",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_EventId_HandlerId",
                table: "EventHandlerRelationships",
                columns: new[] { "EventId", "HandlerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_HandlerId",
                table: "EventHandlerRelationships",
                column: "HandlerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_IsDeleted",
                table: "EventHandlerRelationships",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_RelationshipType",
                table: "EventHandlerRelationships",
                column: "EventHandlerRelationshipType");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_Source",
                table: "EventHandlerRelationships",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerRelationships_UpdatedAt",
                table: "EventHandlerRelationships",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_ArgumentsHash",
                table: "EventHandlers",
                column: "ArgumentsHash");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_FilePath",
                table: "EventHandlers",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_FullName",
                table: "EventHandlers",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_IsDeleted",
                table: "EventHandlers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_IsPublic",
                table: "EventHandlers",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlers_LastModified",
                table: "EventHandlers",
                column: "LastModified");

            migrationBuilder.CreateIndex(
                name: "IX_UnityEvents_ArgumentsHash",
                table: "UnityEvents",
                column: "ArgumentsHash");

            migrationBuilder.CreateIndex(
                name: "IX_UnityEvents_FilePath",
                table: "UnityEvents",
                column: "FilePath");

            migrationBuilder.CreateIndex(
                name: "IX_UnityEvents_FullName",
                table: "UnityEvents",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnityEvents_IsDeleted",
                table: "UnityEvents",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_UnityEvents_LastModified",
                table: "UnityEvents",
                column: "LastModified");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserChats_ChatId",
                table: "Messages",
                column: "ChatId",
                principalTable: "UserChats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserChats_ChatId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "EventHandlerRelationships");

            migrationBuilder.DropTable(
                name: "EventHandlers");

            migrationBuilder.DropTable(
                name: "UnityEvents");

            migrationBuilder.DropIndex(
                name: "IX_Users_Name",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.AddColumn<string>(
                name: "UserChatId",
                table: "Messages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserChatId",
                table: "Messages",
                column: "UserChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserChats_UserChatId",
                table: "Messages",
                column: "UserChatId",
                principalTable: "UserChats",
                principalColumn: "Id");
        }
    }
}
