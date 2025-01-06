using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalMe.Migrations
{
    /// <inheritdoc />
    public partial class MessageLiked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Liked",
                table: "Messages",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Liked",
                table: "Messages");
        }
    }
}
