using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Up.Bsky.PostBot.Migrations
{
    /// <inheritdoc />
    public partial class TrackUserDID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserDid",
                table: "SeenPosts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SeenPosts_UserDid",
                table: "SeenPosts",
                column: "UserDid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SeenPosts_UserDid",
                table: "SeenPosts");

            migrationBuilder.DropColumn(
                name: "UserDid",
                table: "SeenPosts");
        }
    }
}
