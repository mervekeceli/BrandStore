using Microsoft.EntityFrameworkCore.Migrations;

namespace BrandStore.Migrations
{
    public partial class adminMarkaEslesmesi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Brands",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lastname",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_ApplicationUserId",
                table: "Brands",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_AspNetUsers_ApplicationUserId",
                table: "Brands",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_AspNetUsers_ApplicationUserId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_ApplicationUserId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Lastname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");
        }
    }
}
