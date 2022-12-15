using Microsoft.EntityFrameworkCore;

namespace JWTAuth.WebApi.Models
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee>? Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            //OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
