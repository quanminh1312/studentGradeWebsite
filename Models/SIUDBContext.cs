using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using doancoso.Models;

namespace doancoso.Models
{
    public class SIUDBContext: IdentityDbContext<User>
    {
        public SIUDBContext(DbContextOptions<SIUDBContext> options) : base(options)
        {
        }
        public DbSet<doancoso.Models.Account> Accounts { get; set; } = default!;
        public DbSet<doancoso.Models.Class> Class { get; set; } = default!;
        public DbSet<doancoso.Models.Major> Major { get; set; } = default!;
        public DbSet<doancoso.Models.Grade> Grade { get; set; } = default!;
        public DbSet<doancoso.Models.Semester> Semester { get; set; } = default!;
        public DbSet<doancoso.Models.Student> Student { get; set; } = default!;
        public DbSet<doancoso.Models.Teacher> Teacher { get; set; } = default!;
        public DbSet<doancoso.Models.SemesGradesStudent> SemesGradesStudent { get; set; } = default!;
    }
}
