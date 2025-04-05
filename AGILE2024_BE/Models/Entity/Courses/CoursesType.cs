using System.ComponentModel.DataAnnotations.Schema;

namespace AGILE2024_BE.Models.Entity.Courses
{
    public class CoursesType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string DescriptionType { get; set; }
    }
}
