using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Courses
{
    public class CourseEmployee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Course) + "Id")]
        public Course Course { get; set; }
        [ForeignKey(nameof(Employee) + "Id")]
        public EmployeeCard Employee { get; set; }
        [ForeignKey(nameof(State) + "Id")]
        public CourseState State { get; set; }
        public DateOnly? CompletedDate { get; set; }
    }
}
