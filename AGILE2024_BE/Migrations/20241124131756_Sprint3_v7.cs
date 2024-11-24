using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_v7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_recipientid",
                table: "FeedbackAnswers");

            migrationBuilder.RenameColumn(
                name: "recipientid",
                table: "FeedbackAnswers",
                newName: "FeedbackRecipientId");

            migrationBuilder.RenameIndex(
                name: "IX_FeedbackAnswers_recipientid",
                table: "FeedbackAnswers",
                newName: "IX_FeedbackAnswers_FeedbackRecipientId");

            migrationBuilder.AddColumn<bool>(
                name: "isReadBySender",
                table: "FeedbackRecipients",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_FeedbackRecipientId",
                table: "FeedbackAnswers",
                column: "FeedbackRecipientId",
                principalTable: "FeedbackRecipients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_FeedbackRecipientId",
                table: "FeedbackAnswers");

            migrationBuilder.DropColumn(
                name: "isReadBySender",
                table: "FeedbackRecipients");

            migrationBuilder.RenameColumn(
                name: "FeedbackRecipientId",
                table: "FeedbackAnswers",
                newName: "recipientid");

            migrationBuilder.RenameIndex(
                name: "IX_FeedbackAnswers_FeedbackRecipientId",
                table: "FeedbackAnswers",
                newName: "IX_FeedbackAnswers_recipientid");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackAnswers_FeedbackRecipients_recipientid",
                table: "FeedbackAnswers",
                column: "recipientid",
                principalTable: "FeedbackRecipients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
