using CyberQuiz.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CyberQuiz.DAL.Data;

public class CyberQuizDbContext : IdentityDbContext<IdentityUser>
{
    public CyberQuizDbContext(DbContextOptions<CyberQuizDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SubCategory> SubCategories => Set<SubCategory>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<UserResult> UserResults => Set<UserResult>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Category
        builder.Entity<Category>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Name).IsUnique();
        });

        //SubCategory
        builder.Entity<SubCategory>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Category)
             .WithMany(c => c.SubCategories)
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Cascade);

            
            e.HasIndex(x => new { x.CategoryId, x.Order }).IsUnique();
        });

        //Question
        builder.Entity<Question>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.SubCategory)
             .WithMany(sc => sc.Questions)
             .HasForeignKey(x => x.SubCategoryId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // AnswerOption 
        builder.Entity<AnswerOption>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Question)
             .WithMany(q => q.AnswerOptions)
             .HasForeignKey(x => x.QuestionId)
             .OnDelete(DeleteBehavior.Cascade);

            
            e.HasIndex(x => new { x.QuestionId, x.Text }).IsUnique();
        });

        // UserResult
        builder.Entity<UserResult>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.SubCategory)
             .WithMany()
             .HasForeignKey(x => x.SubCategoryId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Question)
             .WithMany()
             .HasForeignKey(x => x.QuestionId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.SelectedAnswerOption)
             .WithMany()
             .HasForeignKey(x => x.SelectedAnswerOptionId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => new { x.UserId, x.SubCategoryId });
        });
    }
}