﻿// <auto-generated />
using System;
using AGILE2024_BE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AGILE2024_BE.Migrations
{
    [DbContext(typeof(AgileDBContext))]
    [Migration("20241120120836_Sprint3_v5")]
    partial class Sprint3_v5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("AGILE2024_BE.Models.ContractType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("ContractTypes");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastEdited")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("OrganizationId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("ParentDepartmentId")
                        .HasColumnType("char(36)");

                    b.Property<string>("SuperiorId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("ParentDepartmentId");

                    b.HasIndex("SuperiorId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.EmployeeCard", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("ContractTypeId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DepartmentId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("LastEdited")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("LevelId")
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("LocationId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("StartWorkDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<int?>("WorkPercentage")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContractTypeId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("LevelId");

                    b.HasIndex("LocationId");

                    b.HasIndex("UserId");

                    b.ToTable("EmployeeCards");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackAnswer", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FeedbackQuestionId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("answeredDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("FeedbackQuestionId");

                    b.ToTable("FeedbackAnswers");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackQuestion", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FeedbackRequestId")
                        .HasColumnType("char(36)");

                    b.Property<int>("order")
                        .HasColumnType("int");

                    b.Property<string>("text")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("FeedbackRequestId");

                    b.ToTable("FeedbackQuestions");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackRecipient", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("EmployeeCardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FeedbackRequestId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("isRead")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("recievedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("id");

                    b.HasIndex("EmployeeCardId");

                    b.HasIndex("FeedbackRequestId");

                    b.ToTable("FeedbackRecipients");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackRequest", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("EmployeeCardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("FeedbackRequestStatusId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("sentAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("EmployeeCardId");

                    b.HasIndex("FeedbackRequestStatusId");

                    b.ToTable("FeedbackRequests");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackRequestStatus", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("FeedbackRequestStatuses");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Goal", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("EmployeeCardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GoalCategoryId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GoalStatusId")
                        .HasColumnType("char(36)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("dueDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("finishedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("fullfilmentRate")
                        .HasColumnType("int");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("EmployeeCardId");

                    b.HasIndex("GoalCategoryId");

                    b.HasIndex("GoalStatusId");

                    b.ToTable("Goals");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.GoalAssignment", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("EmployeeCardId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GoalId")
                        .HasColumnType("char(36)");

                    b.HasKey("id");

                    b.HasIndex("EmployeeCardId");

                    b.HasIndex("GoalId");

                    b.ToTable("GoalAssignments");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.GoalCategory", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("GoalCategory");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.GoalStatus", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.ToTable("GoalStatuses");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("ProfilePicLink")
                        .HasColumnType("longtext");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("RefreshTokenExpiry")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<string>("SuperiorId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Title_after")
                        .HasColumnType("longtext");

                    b.Property<string>("Title_before")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.HasIndex("SuperiorId");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("AGILE2024_BE.Models.JobPosition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastEdited")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("JobPositions");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Level", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid?>("JobPositionId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("JobPositionId");

                    b.ToTable("Levels");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Location", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Adress")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastEdited")
                        .HasColumnType("datetime(6)");

                    b.Property<double?>("Latitude")
                        .HasColumnType("double");

                    b.Property<double?>("Longitude")
                        .HasColumnType("double");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("LastEdited")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("LocationId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Review", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("EmployeeCardId")
                        .HasColumnType("char(36)");

                    b.Property<int>("counter")
                        .HasColumnType("int");

                    b.Property<DateTime>("createDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("employeeEndDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("endDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("superiorEndDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("id");

                    b.HasIndex("EmployeeCardId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.ReviewQuestion", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("ReviewRecipientId")
                        .HasColumnType("char(36)");

                    b.Property<string>("employeeDescription")
                        .HasColumnType("longtext");

                    b.Property<string>("superiorDescription")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("ReviewRecipientId");

                    b.ToTable("ReviewQuestions");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.ReviewRecipient", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<Guid>("GoalAssignmentId")
                        .HasColumnType("char(36)");

                    b.Property<string>("employeeDescription")
                        .HasColumnType("longtext");

                    b.Property<string>("superiorDescription")
                        .HasColumnType("longtext");

                    b.HasKey("id");

                    b.HasIndex("GoalAssignmentId");

                    b.ToTable("ReviewRecipents");
                });

            modelBuilder.Entity("JobPositionOrganization", b =>
                {
                    b.Property<Guid>("JobPositionsId")
                        .HasColumnType("char(36)");

                    b.Property<Guid>("OrganizationsId")
                        .HasColumnType("char(36)");

                    b.HasKey("JobPositionsId", "OrganizationsId");

                    b.HasIndex("OrganizationsId");

                    b.ToTable("JobPositionOrganization");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Department", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Organization", "Organization")
                        .WithMany("RelatedDepartments")
                        .HasForeignKey("OrganizationId");

                    b.HasOne("AGILE2024_BE.Models.Department", "ParentDepartment")
                        .WithMany()
                        .HasForeignKey("ParentDepartmentId");

                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", "Superior")
                        .WithMany()
                        .HasForeignKey("SuperiorId");

                    b.Navigation("Organization");

                    b.Navigation("ParentDepartment");

                    b.Navigation("Superior");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.EmployeeCard", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.ContractType", "ContractType")
                        .WithMany()
                        .HasForeignKey("ContractTypeId");

                    b.HasOne("AGILE2024_BE.Models.Department", "Department")
                        .WithMany("EmployeeCards")
                        .HasForeignKey("DepartmentId");

                    b.HasOne("AGILE2024_BE.Models.Level", "Level")
                        .WithMany("EmployeeCards")
                        .HasForeignKey("LevelId");

                    b.HasOne("AGILE2024_BE.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("ContractType");

                    b.Navigation("Department");

                    b.Navigation("Level");

                    b.Navigation("Location");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackAnswer", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.FeedbackQuestion", "request")
                        .WithMany()
                        .HasForeignKey("FeedbackQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("request");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackQuestion", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.FeedbackRequest", "request")
                        .WithMany()
                        .HasForeignKey("FeedbackRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("request");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackRecipient", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.EmployeeCard", "employee")
                        .WithMany()
                        .HasForeignKey("EmployeeCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.FeedbackRequest", "feedbackRequest")
                        .WithMany()
                        .HasForeignKey("FeedbackRequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("employee");

                    b.Navigation("feedbackRequest");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.FeedbackRequest", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.EmployeeCard", "sender")
                        .WithMany()
                        .HasForeignKey("EmployeeCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.FeedbackRequestStatus", "status")
                        .WithMany()
                        .HasForeignKey("FeedbackRequestStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("sender");

                    b.Navigation("status");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Goal", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.EmployeeCard", "employee")
                        .WithMany()
                        .HasForeignKey("EmployeeCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.GoalCategory", "category")
                        .WithMany()
                        .HasForeignKey("GoalCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.GoalStatus", "status")
                        .WithMany()
                        .HasForeignKey("GoalStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("category");

                    b.Navigation("employee");

                    b.Navigation("status");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.GoalAssignment", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.EmployeeCard", "employee")
                        .WithMany()
                        .HasForeignKey("EmployeeCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.Goal", "goal")
                        .WithMany()
                        .HasForeignKey("GoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("employee");

                    b.Navigation("goal");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", "Superior")
                        .WithMany()
                        .HasForeignKey("SuperiorId");

                    b.Navigation("Superior");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Level", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.JobPosition", "JobPosition")
                        .WithMany("Levels")
                        .HasForeignKey("JobPositionId");

                    b.Navigation("JobPosition");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Organization", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Review", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.EmployeeCard", "sender")
                        .WithMany()
                        .HasForeignKey("EmployeeCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("sender");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.ReviewQuestion", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.ReviewRecipient", "goalAssignment")
                        .WithMany()
                        .HasForeignKey("ReviewRecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("goalAssignment");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.ReviewRecipient", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.GoalAssignment", "goalAssignment")
                        .WithMany()
                        .HasForeignKey("GoalAssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("goalAssignment");
                });

            modelBuilder.Entity("JobPositionOrganization", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.JobPosition", null)
                        .WithMany()
                        .HasForeignKey("JobPositionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("AGILE2024_BE.Models.Identity.ExtendedIdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Department", b =>
                {
                    b.Navigation("EmployeeCards");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.JobPosition", b =>
                {
                    b.Navigation("Levels");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Level", b =>
                {
                    b.Navigation("EmployeeCards");
                });

            modelBuilder.Entity("AGILE2024_BE.Models.Organization", b =>
                {
                    b.Navigation("RelatedDepartments");
                });
#pragma warning restore 612, 618
        }
    }
}
