using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Smart_Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityAndSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reviews_ServiceRequests_ServiceRequestId",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_services_ServiceId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_services_AspNetUsers_ProviderId",
                table: "services");

            migrationBuilder.DropForeignKey(
                name: "FK_services_ServiceCategories_CategoryId",
                table: "services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_services",
                table: "services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reviews",
                table: "reviews");

            migrationBuilder.RenameTable(
                name: "services",
                newName: "Services");

            migrationBuilder.RenameTable(
                name: "reviews",
                newName: "Reviews");

            migrationBuilder.RenameColumn(
                name: "IsAvaiable",
                table: "Services",
                newName: "IsAvailable");

            migrationBuilder.RenameIndex(
                name: "IX_services_ProviderId",
                table: "Services",
                newName: "IX_Services_ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_services_CategoryId",
                table: "Services",
                newName: "IX_Services_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_ServiceRequestId",
                table: "Reviews",
                newName: "IX_Reviews_ServiceRequestId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ServiceRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Reviews",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateAt",
                table: "Reviews",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Services",
                table: "Services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_ServiceRequests_ServiceRequestId",
                table: "Reviews",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Services_ServiceId",
                table: "ServiceRequests",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_AspNetUsers_ProviderId",
                table: "Services",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceCategories_CategoryId",
                table: "Services",
                column: "CategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_ServiceRequests_ServiceRequestId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Services_ServiceId",
                table: "ServiceRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_AspNetUsers_ProviderId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceCategories_CategoryId",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Services",
                table: "Services");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "services");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "reviews");

            migrationBuilder.RenameColumn(
                name: "IsAvailable",
                table: "services",
                newName: "IsAvaiable");

            migrationBuilder.RenameIndex(
                name: "IX_Services_ProviderId",
                table: "services",
                newName: "IX_services_ProviderId");

            migrationBuilder.RenameIndex(
                name: "IX_Services_CategoryId",
                table: "services",
                newName: "IX_services_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ServiceRequestId",
                table: "reviews",
                newName: "IX_reviews_ServiceRequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_services",
                table: "services",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reviews",
                table: "reviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_ServiceRequests_ServiceRequestId",
                table: "reviews",
                column: "ServiceRequestId",
                principalTable: "ServiceRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_services_ServiceId",
                table: "ServiceRequests",
                column: "ServiceId",
                principalTable: "services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_services_AspNetUsers_ProviderId",
                table: "services",
                column: "ProviderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_services_ServiceCategories_CategoryId",
                table: "services",
                column: "CategoryId",
                principalTable: "ServiceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
