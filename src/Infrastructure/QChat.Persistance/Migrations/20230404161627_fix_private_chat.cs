using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QChat.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class fix_private_chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId2",
                table: "Chats",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId1",
                table: "Chats",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId2",
                table: "Chats",
                column: "UserId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId1",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId2",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserId2",
                table: "Chats");
        }
    }
}
