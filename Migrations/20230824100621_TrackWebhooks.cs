using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Up.Bsky.PostBot.Migrations
{
    /// <inheritdoc />
    public partial class TrackWebhooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WebhookId",
                table: "DiscordChannels",
                type: "numeric(20,0)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebhookId",
                table: "DiscordChannels");
        }
    }
}
