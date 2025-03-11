using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "referencedItemId",
                table: "Notifications",
                newName: "ReferencedItemId");

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewId",
                table: "ReviewRecipents",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<bool>(
                name: "isSavedEmployeeDesc",
                table: "ReviewRecipents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSavedSuperiorDesc",
                table: "ReviewRecipents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSentEmployeeDesc",
                table: "ReviewRecipents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSentSuperiorDesc",
                table: "ReviewRecipents",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSavedEmployeeDesc",
                table: "ReviewQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSavedSuperiorDesc",
                table: "ReviewQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSentEmployeeDesc",
                table: "ReviewQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isSentSuperiorDesc",
                table: "ReviewQuestions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipents_ReviewId",
                table: "ReviewRecipents",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewRecipents_Reviews_ReviewId",
                table: "ReviewRecipents",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewRecipents_Reviews_ReviewId",
                table: "ReviewRecipents");

            migrationBuilder.DropIndex(
                name: "IX_ReviewRecipents_ReviewId",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "isSavedEmployeeDesc",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "isSavedSuperiorDesc",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "isSentEmployeeDesc",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "isSentSuperiorDesc",
                table: "ReviewRecipents");

            migrationBuilder.DropColumn(
                name: "isSavedEmployeeDesc",
                table: "ReviewQuestions");

            migrationBuilder.DropColumn(
                name: "isSavedSuperiorDesc",
                table: "ReviewQuestions");

            migrationBuilder.DropColumn(
                name: "isSentEmployeeDesc",
                table: "ReviewQuestions");

            migrationBuilder.DropColumn(
                name: "isSentSuperiorDesc",
                table: "ReviewQuestions");

            migrationBuilder.RenameColumn(
                name: "ReferencedItemId",
                table: "Notifications",
                newName: "referencedItemId");
        }
    }
}
