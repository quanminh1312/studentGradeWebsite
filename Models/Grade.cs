using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Description { get; set; }

        public required int maxGrade { get; set;}
    }
}
