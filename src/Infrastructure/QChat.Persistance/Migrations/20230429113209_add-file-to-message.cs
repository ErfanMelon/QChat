using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QChat.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class addfiletomessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileSrc",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSrc",
                table: "Messages");
        }
    }
}
