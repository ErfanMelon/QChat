using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QChat.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addprivateChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChatType",
                table: "Chats",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatType",
                table: "Chats");
        }
    }
}
