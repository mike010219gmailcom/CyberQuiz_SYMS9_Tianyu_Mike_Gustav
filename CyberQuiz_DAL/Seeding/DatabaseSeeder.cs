using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CyberQuiz.DAL.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        CyberQuizDbContext db,
        UserManager<IdentityUser> userManager,
        CancellationToken ct = default)
    {
        // Kör migrations
        await db.Database.MigrateAsync(ct);

        // Default user
        await EnsureDefaultUserAsync(userManager);

        // Seed domändata bara om tom DB
        if (await db.Categories.AnyAsync(ct))
            return;

        var categories = new List<Category>
        {
            new() { Name = "Grundläggande säkerhet" },
            new() { Name = "Nätverk & attacker" },
            new() { Name = "Webbsäkerhet" }
        };
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync(ct);

        var subCategories = new List<SubCategory>
        {
            new() { CategoryId = categories[0].Id, Name = "Lösenord & MFA", Order = 1 },
            new() { CategoryId = categories[0].Id, Name = "Social engineering", Order = 2 },

            new() { CategoryId = categories[1].Id, Name = "Nätverksgrunder", Order = 1 },
            new() { CategoryId = categories[1].Id, Name = "Vanliga attacker", Order = 2 },

            new() { CategoryId = categories[2].Id, Name = "OWASP basics", Order = 1 },
            new() { CategoryId = categories[2].Id, Name = "Injection", Order = 2 },
        };
        db.SubCategories.AddRange(subCategories);
        await db.SaveChangesAsync(ct);

        var q1 = new Question
        {
            SubCategoryId = subCategories[0].Id,
            Text = "Vilket alternativ beskriver bäst en stark lösenordsstrategi?"
        };

        var q2 = new Question
        {
            SubCategoryId = subCategories[0].Id,
            Text = "Vad är syftet med MFA (Multi-Factor Authentication)?"
        };

        var q3 = new Question
        {
            SubCategoryId = subCategories[5].Id,
            Text = "Vilket är ett vanligt exempel på SQL Injection?"
        };

        db.Questions.AddRange(q1, q2, q3);
        await db.SaveChangesAsync(ct);

        db.AnswerOptions.AddRange(
            new AnswerOption { QuestionId = q1.Id, Text = "Samma lösenord överallt, men långt", IsCorrect = false },
            new AnswerOption { QuestionId = q1.Id, Text = "Unikt lösenord per tjänst + lösenordshanterare", IsCorrect = true },
            new AnswerOption { QuestionId = q1.Id, Text = "Personliga ord som är lätta att minnas", IsCorrect = false },
            new AnswerOption { QuestionId = q1.Id, Text = "Byta lösenord varje vecka oavsett", IsCorrect = false },

            new AnswerOption { QuestionId = q2.Id, Text = "Att kryptera databasen", IsCorrect = false },
            new AnswerOption { QuestionId = q2.Id, Text = "Att lägga till fler oberoende verifieringssteg", IsCorrect = true },
            new AnswerOption { QuestionId = q2.Id, Text = "Att göra inloggning snabbare", IsCorrect = false },
            new AnswerOption { QuestionId = q2.Id, Text = "Att ersätta lösenord helt i alla system", IsCorrect = false },

            new AnswerOption { QuestionId = q3.Id, Text = "Att lägga till extra CSS-klasser i HTML", IsCorrect = false },
            new AnswerOption { QuestionId = q3.Id, Text = "Att injicera SQL via indata för att manipulera queries", IsCorrect = true },
            new AnswerOption { QuestionId = q3.Id, Text = "Att blockera portar i brandväggen", IsCorrect = false },
            new AnswerOption { QuestionId = q3.Id, Text = "Att använda HTTPS", IsCorrect = false }
        );

        await db.SaveChangesAsync(ct);
    }

    private static async Task EnsureDefaultUserAsync(UserManager<IdentityUser> userManager)
    {
        const string username = "user";
        const string password = "Password1234!";

        var existing = await userManager.FindByNameAsync(username);
        if (existing != null) return;

        var user = new IdentityUser
        {
            UserName = username,
            Email = "user@cyberquiz.local",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException(msg);
        }
    }
}