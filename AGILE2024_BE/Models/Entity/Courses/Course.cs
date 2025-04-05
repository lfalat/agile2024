using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Courses
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateOnly ExpirationDate { get; set; }
        public string Name { get; set; }
        [ForeignKey(nameof(CreatedEmployee) + "Id")]
        public EmployeeCard CreatedEmployee { get; set; }
        [ForeignKey(nameof(Type) + "Id")]
        public CoursesType Type { get; set; }
        public int Version { get; set; }
        public string DetailDescription { get; set; }
    }
}
