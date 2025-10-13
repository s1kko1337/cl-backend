using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cl_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "ProductReviews",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProductReviews",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "ProductReviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProductReviews");
        }
    }
}
