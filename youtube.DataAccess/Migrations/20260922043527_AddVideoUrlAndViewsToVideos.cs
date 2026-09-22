using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace youtube.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlAndViewsToVideos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_likesDislikes_AspNetUsers_AppUserId",
                table: "likesDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_likesDislikes_videos_VideoId",
                table: "likesDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_SubScription_AspNetUsers_AppUserId",
                table: "SubScription");

            migrationBuilder.DropForeignKey(
                name: "FK_SubScription_Channals_ChannalId",
                table: "SubScription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_likesDislikes",
                table: "likesDislikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubScription",
                table: "SubScription");

            migrationBuilder.RenameTable(
                name: "likesDislikes",
                newName: "LikesDislikes");

            migrationBuilder.RenameTable(
                name: "SubScription",
                newName: "Subscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_likesDislikes_VideoId",
                table: "LikesDislikes",
                newName: "IX_LikesDislikes_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_SubScription_ChannalId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_ChannalId");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "videos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LikesDislikes",
                table: "LikesDislikes",
                columns: new[] { "AppUserId", "VideoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                columns: new[] { "AppUserId", "ChannalId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LikesDislikes_AspNetUsers_AppUserId",
                table: "LikesDislikes",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LikesDislikes_videos_VideoId",
                table: "LikesDislikes",
                column: "VideoId",
                principalTable: "videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_AspNetUsers_AppUserId",
                table: "Subscriptions",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Channals_ChannalId",
                table: "Subscriptions",
                column: "ChannalId",
                principalTable: "Channals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikesDislikes_AspNetUsers_AppUserId",
                table: "LikesDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_LikesDislikes_videos_VideoId",
                table: "LikesDislikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_AspNetUsers_AppUserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Channals_ChannalId",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LikesDislikes",
                table: "LikesDislikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "videos");

            migrationBuilder.DropColumn(
                name: "Views",
                table: "videos");

            migrationBuilder.RenameTable(
                name: "LikesDislikes",
                newName: "likesDislikes");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "SubScription");

            migrationBuilder.RenameIndex(
                name: "IX_LikesDislikes_VideoId",
                table: "likesDislikes",
                newName: "IX_likesDislikes_VideoId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_ChannalId",
                table: "SubScription",
                newName: "IX_SubScription_ChannalId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_likesDislikes",
                table: "likesDislikes",
                columns: new[] { "AppUserId", "VideoId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubScription",
                table: "SubScription",
                columns: new[] { "AppUserId", "ChannalId" });

            migrationBuilder.AddForeignKey(
                name: "FK_likesDislikes_AspNetUsers_AppUserId",
                table: "likesDislikes",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_likesDislikes_videos_VideoId",
                table: "likesDislikes",
                column: "VideoId",
                principalTable: "videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SubScription_AspNetUsers_AppUserId",
                table: "SubScription",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubScription_Channals_ChannalId",
                table: "SubScription",
                column: "ChannalId",
                principalTable: "Channals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
