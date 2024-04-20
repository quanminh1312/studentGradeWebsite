using System.ComponentModel.DataAnnotations;
namespace doancoso.Models
{
    public class SemesGradesStudent
    {
        [Key]
        public int Id { get; set; }
        public required List<int> GradeIds { get; set; }
        public required List<int> GradeNums { get; set; }
        public int TotalGrade { get; set; }
        public int SemesterId { get; set; }
        public int StudentId { get; set; }
        public int? TeacherId { get; set; }
        public bool Pending { get; set; }
        public Semester? semester { get; set; }
        public Student? student { get; set; }
        public List<Grade>? Grades { get; set; }
        public Teacher? teacher { get; set; }
    }
}
