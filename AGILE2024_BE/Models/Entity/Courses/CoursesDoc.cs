using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Courses
{
    public class CoursesDoc
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [ForeignKey(nameof(Course) + "Id")]
        public Course Course { get; set; }
        public string DescriptionDocs { get; set; }
        public string FilePath { get; set; }
    }
}
