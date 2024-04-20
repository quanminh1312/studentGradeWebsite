using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Degree { get; set; }
        public int? MajorId { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public Major? Major { get; set; }
    }
}
