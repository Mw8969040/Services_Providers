using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddYearsOfExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "ProviderProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "ProviderProfiles");
        }
    }
}
