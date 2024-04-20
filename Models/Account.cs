using System.ComponentModel.DataAnnotations;

namespace doancoso.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }
        public bool? Pending { get; set; }
        public Account(User appUser)
        {
            Username = appUser.UserName;
            Email = appUser.Email;
            Password = appUser.PasswordHash;
            Pending = appUser.Pending;
            Role = appUser.Role;
        }
        public Account()
        {
            Pending = false;
        }
    }
}
