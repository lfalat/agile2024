﻿using AGILE2024_BE.Models;
using AGILE2024_BE.Models.Entity.Adaptation;
using AGILE2024_BE.Models.Entity.Courses;
using AGILE2024_BE.Models.Entity.SuccessionPlan;
using AGILE2024_BE.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AGILE2024_BE.Data
{
    /// <summary>
    /// Tato trieda služi ako databazovy context pre talent hub aplikaciu
    /// </summary>
    public class AgileDBContext : IdentityDbContext<ExtendedIdentityUser, IdentityRole, string>
    {
        public AgileDBContext(DbContextOptions options) : base(options)
        {

        }

        //Tento region služi na zadefinovane tabuliek v databaze
        //DbSet = tabuľka, typ v zobačikoch definuje stlpce v databaze
        #region TABLES
        //Employee Cards
        public DbSet<EmployeeCard> EmployeeCards { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        //Departments and Organizations
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Department> Departments { get; set; }
        //Job Positions
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<Level> Levels { get; set; }
        //Locations
        public DbSet<Location> Locations { get; set; }
        //Feedback
        public DbSet<FeedbackAnswer> FeedbackAnswers { get; set; }
        public DbSet<FeedbackQuestion> FeedbackQuestions { get; set; }
        public DbSet<FeedbackRequest> FeedbackRequests { get; set; }
        public DbSet<FeedbackRequestStatus> FeedbackRequestStatuses { get; set; }
        public DbSet<FeedbackRecipient> FeedbackRecipients { get; set; }
        //Goals
        public DbSet<Goal> Goals { get; set; }
        public DbSet<GoalAssignment> GoalAssignments { get; set; }
        public DbSet<GoalStatus> GoalStatuses { get; set; }
        public DbSet<GoalCategory> GoalCategory{ get; set; } 
        //Reviews
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewQuestion> ReviewQuestions { get; set; }
        public DbSet<ReviewRecipient> ReviewRecipents { get; set; }
        //Notifications
        public DbSet<Notification> Notifications { get; set; }
        //Succession Plan   
        public DbSet<SuccessionPlan> SuccessionPlans { get; set; }
        public DbSet<SuccesionSkills> SuccesionSkills { get; set; }
        public DbSet<ReadyStatus> ReadyStatuses { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        //Courses
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEmployee> CourseEmployees { get; set; }
        public DbSet<CoursesDoc> CoursesDocs { get; set; }
        public DbSet<CourseState> CourseStates { get; set; }
        public DbSet<CoursesType> CoursesTypes { get; set; }
        //Adaptations
        public DbSet<Adaptation> Adaptations { get; set; }
        public DbSet<AdaptationDoc> AdaptationDocs { get; set; }
        public DbSet<AdaptationState> AdaptationStates { get; set; }
        public DbSet<AdaptationTask> AdaptationTasks { get; set; }
        #endregion
    }
}
