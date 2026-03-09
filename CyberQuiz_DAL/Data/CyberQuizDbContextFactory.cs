using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using CyberQuiz.DAL.Data;

namespace CyberQuiz_DAL.Data;


public class CyberQuizDbContextFactory : IDesignTimeDbContextFactory<CyberQuizDbContext>
{
    public CyberQuizDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CyberQuizDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=CyberQuizDb;Trusted_Connection=True;MultipleActiveResultSets=true");

        return new CyberQuizDbContext(optionsBuilder.Options);
    }
}