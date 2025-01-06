using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalMe.Migrations
{
    /// <inheritdoc />
    public partial class ConversationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReceiverConversationIsActive",
                table: "Conversations",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UserConversationIsActive",
                table: "Conversations",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverConversationIsActive",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "UserConversationIsActive",
                table: "Conversations");
        }
    }
}
