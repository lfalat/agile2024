using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_v5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SuperiorId",
                table: "Departments",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_SuperiorId",
                table: "Departments",
                column: "SuperiorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_SuperiorId",
                table: "Departments",
                column: "SuperiorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_SuperiorId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_SuperiorId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "SuperiorId",
                table: "Departments");
        }
    }
}
