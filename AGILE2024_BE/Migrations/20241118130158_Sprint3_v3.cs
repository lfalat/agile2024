using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fullfilmentDate",
                table: "Goals");

            migrationBuilder.AddColumn<int>(
                name: "fullfilmentRate",
                table: "Goals",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fullfilmentRate",
                table: "Goals");

            migrationBuilder.AddColumn<DateTime>(
                name: "fullfilmentDate",
                table: "Goals",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
