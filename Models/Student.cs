using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int ClassId { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public Class? Class { get; set; }

    }
}
