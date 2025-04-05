using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    /// <inheritdoc />
    public partial class Sprint6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdaptationStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionState = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdaptationStates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourseStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionState = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CoursesTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Adaptations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ReadyDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedEmployeeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adaptations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adaptations_AdaptationStates_StateId",
                        column: x => x.StateId,
                        principalTable: "AdaptationStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Adaptations_EmployeeCards_CreatedEmployeeId",
                        column: x => x.CreatedEmployeeId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Adaptations_EmployeeCards_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedEmployeeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    TypeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Version = table.Column<int>(type: "int", nullable: false),
                    DetailDescription = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_CoursesTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CoursesTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_EmployeeCards_CreatedEmployeeId",
                        column: x => x.CreatedEmployeeId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdaptationDocs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdaptationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionDocs = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdaptationDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdaptationDocs_Adaptations_AdaptationId",
                        column: x => x.AdaptationId,
                        principalTable: "Adaptations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AdaptationTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AdaptationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionTask = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDone = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FinishDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdaptationTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdaptationTasks_Adaptations_AdaptationId",
                        column: x => x.AdaptationId,
                        principalTable: "Adaptations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CourseEmployees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EmployeeId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    StateId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CompletedDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseEmployees_CourseStates_StateId",
                        column: x => x.StateId,
                        principalTable: "CourseStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEmployees_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEmployees_EmployeeCards_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "EmployeeCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CoursesDocs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CourseId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DescriptionDocs = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FilePath = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursesDocs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoursesDocs_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AdaptationDocs_AdaptationId",
                table: "AdaptationDocs",
                column: "AdaptationId");

            migrationBuilder.CreateIndex(
                name: "IX_Adaptations_CreatedEmployeeId",
                table: "Adaptations",
                column: "CreatedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Adaptations_EmployeeId",
                table: "Adaptations",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Adaptations_StateId",
                table: "Adaptations",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdaptationTasks_AdaptationId",
                table: "AdaptationTasks",
                column: "AdaptationId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEmployees_CourseId",
                table: "CourseEmployees",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEmployees_EmployeeId",
                table: "CourseEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEmployees_StateId",
                table: "CourseEmployees",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedEmployeeId",
                table: "Courses",
                column: "CreatedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TypeId",
                table: "Courses",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CoursesDocs_CourseId",
                table: "CoursesDocs",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdaptationDocs");

            migrationBuilder.DropTable(
                name: "AdaptationTasks");

            migrationBuilder.DropTable(
                name: "CourseEmployees");

            migrationBuilder.DropTable(
                name: "CoursesDocs");

            migrationBuilder.DropTable(
                name: "Adaptations");

            migrationBuilder.DropTable(
                name: "CourseStates");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "AdaptationStates");

            migrationBuilder.DropTable(
                name: "CoursesTypes");
        }
    }
}
