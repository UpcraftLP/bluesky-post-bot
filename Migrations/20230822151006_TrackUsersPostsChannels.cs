using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Up.Bsky.PostBot.Migrations
{
    /// <inheritdoc />
    public partial class TrackUsersPostsChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SeenPosts",
                table: "SeenPosts");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SeenPosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeenPosts",
                table: "SeenPosts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DiscordChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    ServerId = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackedUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Did = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedUsers", x => x.Id);
                    table.UniqueConstraint("AK_TrackedUsers_Did", x => x.Did);
                });

            migrationBuilder.CreateTable(
                name: "BskyUserDiscordChannel",
                columns: table => new
                {
                    DiscordChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackedUsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BskyUserDiscordChannel", x => new { x.DiscordChannelId, x.TrackedUsersId });
                    table.ForeignKey(
                        name: "FK_BskyUserDiscordChannel_DiscordChannels_DiscordChannelId",
                        column: x => x.DiscordChannelId,
                        principalTable: "DiscordChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BskyUserDiscordChannel_TrackedUsers_TrackedUsersId",
                        column: x => x.TrackedUsersId,
                        principalTable: "TrackedUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeenPosts_AtUri",
                table: "SeenPosts",
                column: "AtUri",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BskyUserDiscordChannel_TrackedUsersId",
                table: "BskyUserDiscordChannel",
                column: "TrackedUsersId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordChannels_ChannelId",
                table: "DiscordChannels",
                column: "ChannelId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedUsers_Did",
                table: "TrackedUsers",
                column: "Did",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SeenPosts_TrackedUsers_UserDid",
                table: "SeenPosts",
                column: "UserDid",
                principalTable: "TrackedUsers",
                principalColumn: "Did",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeenPosts_TrackedUsers_UserDid",
                table: "SeenPosts");

            migrationBuilder.DropTable(
                name: "BskyUserDiscordChannel");

            migrationBuilder.DropTable(
                name: "DiscordChannels");

            migrationBuilder.DropTable(
                name: "TrackedUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SeenPosts",
                table: "SeenPosts");

            migrationBuilder.DropIndex(
                name: "IX_SeenPosts_AtUri",
                table: "SeenPosts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SeenPosts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SeenPosts",
                table: "SeenPosts",
                column: "AtUri");
        }
    }
}
