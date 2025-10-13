using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cl_backend.Migrations
{
    /// <inheritdoc />
    public partial class ReviewFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewImageUrl",
                table: "ProductReviews",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewImageUrl",
                table: "ProductReviews");
        }
    }
}
