using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToServiceAndCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ServiceCategories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "services");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ServiceCategories");
        }
    }
}
