using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint5opravasuccession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuccessionPlans_EmployeeCards_EmployeeCardId",
                table: "SuccessionPlans");

            migrationBuilder.DropIndex(
                name: "IX_SuccessionPlans_EmployeeCardId",
                table: "SuccessionPlans");

            migrationBuilder.DropColumn(
                name: "EmployeeCardId",
                table: "SuccessionPlans");

            migrationBuilder.AddColumn<Guid>(
                name: "successorId",
                table: "SuccessionPlans",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessionPlans_successorId",
                table: "SuccessionPlans",
                column: "successorId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuccessionPlans_EmployeeCards_successorId",
                table: "SuccessionPlans",
                column: "successorId",
                principalTable: "EmployeeCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SuccessionPlans_EmployeeCards_successorId",
                table: "SuccessionPlans");

            migrationBuilder.DropIndex(
                name: "IX_SuccessionPlans_successorId",
                table: "SuccessionPlans");

            migrationBuilder.DropColumn(
                name: "successorId",
                table: "SuccessionPlans");

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeCardId",
                table: "SuccessionPlans",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessionPlans_EmployeeCardId",
                table: "SuccessionPlans",
                column: "EmployeeCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_SuccessionPlans_EmployeeCards_EmployeeCardId",
                table: "SuccessionPlans",
                column: "EmployeeCardId",
                principalTable: "EmployeeCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
