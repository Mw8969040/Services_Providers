using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileImagesAndProviderName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "ProviderProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "ProviderProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "CustomerProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "ProviderProfiles");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "CustomerProfiles");
        }
    }
}
