using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_v6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackRequests_FeedbackRequestStatuses_FeedbackRequestStat~",
                table: "FeedbackRequests");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackRequests_FeedbackRequestStatusId",
                table: "FeedbackRequests");

            migrationBuilder.DropColumn(
                name: "FeedbackRequestStatusId",
                table: "FeedbackRequests");

            migrationBuilder.DropColumn(
                name: "sentAt",
                table: "FeedbackRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackRequestStatusId",
                table: "FeedbackRecipients",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "recipientid",
                table: "FeedbackAnswers",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRecipients_FeedbackRequestStatusId",
                table: "FeedbackRecipients",
                column: "FeedbackRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_recipientid",
                table: "FeedbackAnswers",
                column: "recipientid");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_recipientid",
                table: "FeedbackAnswers",
                column: "recipientid",
                principalTable: "FeedbackRecipients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackRecipients_FeedbackRequestStatuses_FeedbackRequestSt~",
                table: "FeedbackRecipients",
                column: "FeedbackRequestStatusId",
                principalTable: "FeedbackRequestStatuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_recipientid",
                table: "FeedbackAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackRecipients_FeedbackRequestStatuses_FeedbackRequestSt~",
                table: "FeedbackRecipients");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackRecipients_FeedbackRequestStatusId",
                table: "FeedbackRecipients");

            migrationBuilder.DropIndex(
                name: "IX_FeedbackAnswers_recipientid",
                table: "FeedbackAnswers");

            migrationBuilder.DropColumn(
                name: "FeedbackRequestStatusId",
                table: "FeedbackRecipients");

            migrationBuilder.DropColumn(
                name: "recipientid",
                table: "FeedbackAnswers");

            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackRequestStatusId",
                table: "FeedbackRequests",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "sentAt",
                table: "FeedbackRequests",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRequests_FeedbackRequestStatusId",
                table: "FeedbackRequests",
                column: "FeedbackRequestStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackRequests_FeedbackRequestStatuses_FeedbackRequestStat~",
                table: "FeedbackRequests",
                column: "FeedbackRequestStatusId",
                principalTable: "FeedbackRequestStatuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
