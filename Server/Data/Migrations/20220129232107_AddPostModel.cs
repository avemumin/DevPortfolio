using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Data.Migrations
{
    public partial class AddPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ThumbnailimagePath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 65536, nullable: false),
                    PublishDate = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Published = table.Column<bool>(type: "bit", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "PostId", "Author", "CategoryId", "Content", "Excerpt", "PublishDate", "Published", "ThumbnailimagePath", "Title" },
                values: new object[,]
                {
                    { 1, "John Doe", 1, "", "This is the excerpt for post 1. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "First post" },
                    { 2, "John Doe", 2, "", "This is the excerpt for post 2. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "Second post" },
                    { 3, "John Doe", 3, "", "This is the excerpt for post 3. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "Third post" },
                    { 4, "John Doe", 1, "", "This is the excerpt for post 4. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "Fourth post" },
                    { 5, "John Doe", 2, "", "This is the excerpt for post 5. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "Fifth post" },
                    { 6, "John Doe", 3, "", "This is the excerpt for post 6. An excerpt is a little extraction from a larger piece of text. Sort of lika a preview ", "29-01-2022 11:21", true, "uploads/placeholder.jpg", "Sixth post" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
