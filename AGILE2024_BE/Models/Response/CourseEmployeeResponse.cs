namespace AGILE2024_BE.Models.Response
{
    public class CourseEmployeeResponse
    {
        public Guid Id { get; set; }

        // Course
        public string CourseName { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public string CourseType { get; set; }
        public int Version { get; set; }
        public string DetailDescription { get; set; }

        // Employee
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        // Course State
        public string CourseState { get; set; }
        public DateOnly? CompletedDate { get; set; }

        // Source info
        public string CreatedBy { get; set; }
        public string Department { get; set; }
    }
}
