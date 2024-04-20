using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int MajorId { get; set; }
        public Major? ClassMajor { get; set; }

    }
}
