using Microsoft.AspNetCore.Identity;

namespace doancoso.Models
{
    public class User: IdentityUser
    {
        public bool? Pending { get; set; }
        public required string Role { get; set; }
    }
}
