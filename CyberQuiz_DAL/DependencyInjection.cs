using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CyberQuiz.DAL;


//IStället för att göra en scope i program.cs
public static class DependencyInjection
{
    public static IServiceCollection AddDal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext (SQL Server LocalDB)
        services.AddDbContext<CyberQuizDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IQuizRepository, QuizRepository>();
        services.AddScoped<IUserResultRepository, UserResultRepository>();

        return services;
    }
}