using Microsoft.EntityFrameworkCore;
namespace Pouyak
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }
        public DbSet<ProfileReportModel> Pouyak { get; set; }
    }
}
