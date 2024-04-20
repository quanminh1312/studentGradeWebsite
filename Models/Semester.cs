using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Semester
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
