using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Up.Bsky.PostBot.Migrations
{
    /// <inheritdoc />
    public partial class TrackUserChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BskyUserDiscordChannel_DiscordChannels_DiscordChannelId",
                table: "BskyUserDiscordChannel");

            migrationBuilder.RenameColumn(
                name: "DiscordChannelId",
                table: "BskyUserDiscordChannel",
                newName: "TrackedInChannelsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BskyUserDiscordChannel_DiscordChannels_TrackedInChannelsId",
                table: "BskyUserDiscordChannel",
                column: "TrackedInChannelsId",
                principalTable: "DiscordChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BskyUserDiscordChannel_DiscordChannels_TrackedInChannelsId",
                table: "BskyUserDiscordChannel");

            migrationBuilder.RenameColumn(
                name: "TrackedInChannelsId",
                table: "BskyUserDiscordChannel",
                newName: "DiscordChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_BskyUserDiscordChannel_DiscordChannels_DiscordChannelId",
                table: "BskyUserDiscordChannel",
                column: "DiscordChannelId",
                principalTable: "DiscordChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
