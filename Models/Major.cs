using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Major
    {
        [Key]
        public int Id { get; set;}
        public required string Name { get; set; }
    }
}
