using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedbackRequestStatuses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRequestStatuses", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GoalCategory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalCategory", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GoalStatuses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalStatuses", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeCardId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    counter = table.Column<int>(type: "int", nullable: false),
                    employeeEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    superiorEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    createDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    endDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reviews_EmployeeCards_EmployeeCardId",
                        column: x => x.EmployeeCardId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackRequests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeCardId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sentAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    FeedbackRequestStatusId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRequests", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackRequests_EmployeeCards_EmployeeCardId",
                        column: x => x.EmployeeCardId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackRequests_FeedbackRequestStatuses_FeedbackRequestStat~",
                        column: x => x.FeedbackRequestStatusId,
                        principalTable: "FeedbackRequestStatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GoalCategoryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GoalStatusId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    dueDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    fullfilmentDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    finishedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    EmployeeCardId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goals", x => x.id);
                    table.ForeignKey(
                        name: "FK_Goals_EmployeeCards_EmployeeCardId",
                        column: x => x.EmployeeCardId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_GoalCategory_GoalCategoryId",
                        column: x => x.GoalCategoryId,
                        principalTable: "GoalCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Goals_GoalStatuses_GoalStatusId",
                        column: x => x.GoalStatusId,
                        principalTable: "GoalStatuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackQuestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FeedbackRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackQuestions_FeedbackRequests_FeedbackRequestId",
                        column: x => x.FeedbackRequestId,
                        principalTable: "FeedbackRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackRecipients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FeedbackRequestId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeCardId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    recievedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    isRead = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRecipients", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackRecipients_EmployeeCards_EmployeeCardId",
                        column: x => x.EmployeeCardId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackRecipients_FeedbackRequests_FeedbackRequestId",
                        column: x => x.FeedbackRequestId,
                        principalTable: "FeedbackRequests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GoalAssignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GoalId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeCardId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalAssignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_GoalAssignments_EmployeeCards_EmployeeCardId",
                        column: x => x.EmployeeCardId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GoalAssignments_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeedbackAnswers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FeedbackQuestionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    text = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    answeredDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackAnswers", x => x.id);
                    table.ForeignKey(
                        name: "FK_FeedbackAnswers_FeedbackQuestions_FeedbackQuestionId",
                        column: x => x.FeedbackQuestionId,
                        principalTable: "FeedbackQuestions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReviewRecipents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GoalAssignmentId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    superiorDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    employeeDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewRecipents", x => x.id);
                    table.ForeignKey(
                        name: "FK_ReviewRecipents_GoalAssignments_GoalAssignmentId",
                        column: x => x.GoalAssignmentId,
                        principalTable: "GoalAssignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReviewQuestions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReviewRecipientId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    superiorDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    employeeDescription = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewQuestions", x => x.id);
                    table.ForeignKey(
                        name: "FK_ReviewQuestions_ReviewRecipents_ReviewRecipientId",
                        column: x => x.ReviewRecipientId,
                        principalTable: "ReviewRecipents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackAnswers_FeedbackQuestionId",
                table: "FeedbackAnswers",
                column: "FeedbackQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackQuestions_FeedbackRequestId",
                table: "FeedbackQuestions",
                column: "FeedbackRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRecipients_EmployeeCardId",
                table: "FeedbackRecipients",
                column: "EmployeeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRecipients_FeedbackRequestId",
                table: "FeedbackRecipients",
                column: "FeedbackRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRequests_EmployeeCardId",
                table: "FeedbackRequests",
                column: "EmployeeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRequests_FeedbackRequestStatusId",
                table: "FeedbackRequests",
                column: "FeedbackRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalAssignments_EmployeeCardId",
                table: "GoalAssignments",
                column: "EmployeeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_GoalAssignments_GoalId",
                table: "GoalAssignments",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_EmployeeCardId",
                table: "Goals",
                column: "EmployeeCardId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalCategoryId",
                table: "Goals",
                column: "GoalCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Goals_GoalStatusId",
                table: "Goals",
                column: "GoalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQuestions_ReviewRecipientId",
                table: "ReviewQuestions",
                column: "ReviewRecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewRecipents_GoalAssignmentId",
                table: "ReviewRecipents",
                column: "GoalAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EmployeeCardId",
                table: "Reviews",
                column: "EmployeeCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackAnswers");

            migrationBuilder.DropTable(
                name: "FeedbackRecipients");

            migrationBuilder.DropTable(
                name: "ReviewQuestions");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "FeedbackQuestions");

            migrationBuilder.DropTable(
                name: "ReviewRecipents");

            migrationBuilder.DropTable(
                name: "FeedbackRequests");

            migrationBuilder.DropTable(
                name: "GoalAssignments");

            migrationBuilder.DropTable(
                name: "FeedbackRequestStatuses");

            migrationBuilder.DropTable(
                name: "Goals");

            migrationBuilder.DropTable(
                name: "GoalCategory");

            migrationBuilder.DropTable(
                name: "GoalStatuses");
        }
    }
}
