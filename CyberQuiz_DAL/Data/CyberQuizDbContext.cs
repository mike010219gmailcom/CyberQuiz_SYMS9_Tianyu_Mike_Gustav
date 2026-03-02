using CyberQuiz.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz_DAL.Data
{
    public class CyberQuizDbContext : DbContext
    {
        public CyberQuizDbContext(DbContextOptions<CyberQuizDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories => Set<Category>();
       
    }
}
