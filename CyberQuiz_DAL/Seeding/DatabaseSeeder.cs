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

        // Lösenord & MFA
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

        // Social engineering
        var q3 = new Question
        {
            SubCategoryId = subCategories[1].Id,
            Text = "Vad är phishing?"
        };
        var q4 = new Question
        {
            SubCategoryId = subCategories[1].Id,
            Text = "Vad kallas en attack där angriparen utger sig för att vara en betrodd person för att lura till sig information?"
        };

        // Nätverksgrunder
        var q5 = new Question
        {
            SubCategoryId = subCategories[2].Id,
            Text = "Vad används en brandvägg till?"
        };
        var q6 = new Question
        {
            SubCategoryId = subCategories[2].Id,
            Text = "Vad är skillnaden mellan HTTP och HTTPS?"
        };

        // Vanliga attacker
        var q7 = new Question
        {
            SubCategoryId = subCategories[3].Id,
            Text = "Vad är en DDoS-attack?"
        };
        var q8 = new Question
        {
            SubCategoryId = subCategories[3].Id,
            Text = "Vad är ett Man-in-the-Middle-angrepp?"
        };

        // OWASP basics
        var q9 = new Question
        {
            SubCategoryId = subCategories[4].Id,
            Text = "Vad är OWASP Top 10?"
        };
        var q10 = new Question
        {
            SubCategoryId = subCategories[4].Id,
            Text = "Vilket av följande är en av de vanligaste sårbarheterna enligt OWASP?"
        };

        // Injection
        var q11 = new Question
        {
            SubCategoryId = subCategories[5].Id,
            Text = "Vilket är ett vanligt exempel på SQL Injection?"
        };
        var q12 = new Question
        {
            SubCategoryId = subCategories[5].Id,
            Text = "Hur skyddar man sig bäst mot SQL Injection?"
        };

        db.Questions.AddRange(q1, q2, q3, q4, q5, q6, q7, q8, q9, q10, q11, q12);
        await db.SaveChangesAsync(ct);

        db.AnswerOptions.AddRange(
            // q1 - Lösenord
            new AnswerOption { QuestionId = q1.Id, Text = "Samma lösenord överallt, men långt", IsCorrect = false },
            new AnswerOption { QuestionId = q1.Id, Text = "Unikt lösenord per tjänst + lösenordshanterare", IsCorrect = true },
            new AnswerOption { QuestionId = q1.Id, Text = "Personliga ord som är lätta att minnas", IsCorrect = false },
            new AnswerOption { QuestionId = q1.Id, Text = "Byta lösenord varje vecka oavsett", IsCorrect = false },

            // q2 - MFA
            new AnswerOption { QuestionId = q2.Id, Text = "Att kryptera databasen", IsCorrect = false },
            new AnswerOption { QuestionId = q2.Id, Text = "Att lägga till fler oberoende verifieringssteg", IsCorrect = true },
            new AnswerOption { QuestionId = q2.Id, Text = "Att göra inloggning snabbare", IsCorrect = false },
            new AnswerOption { QuestionId = q2.Id, Text = "Att ersätta lösenord helt i alla system", IsCorrect = false },

            // q3 - Phishing
            new AnswerOption { QuestionId = q3.Id, Text = "En metod för att kryptera e-post", IsCorrect = false },
            new AnswerOption { QuestionId = q3.Id, Text = "En attack där bedragaren försöker lura dig att lämna ut känslig information via falska meddelanden", IsCorrect = true },
            new AnswerOption { QuestionId = q3.Id, Text = "En typ av brandväggsinställning", IsCorrect = false },
            new AnswerOption { QuestionId = q3.Id, Text = "Ett verktyg för lösenordshantering", IsCorrect = false },

            // q4 - Social engineering
            new AnswerOption { QuestionId = q4.Id, Text = "Brute force", IsCorrect = false },
            new AnswerOption { QuestionId = q4.Id, Text = "Pretexting", IsCorrect = true },
            new AnswerOption { QuestionId = q4.Id, Text = "Port scanning", IsCorrect = false },
            new AnswerOption { QuestionId = q4.Id, Text = "SQL Injection", IsCorrect = false },

            // q5 - Brandvägg
            new AnswerOption { QuestionId = q5.Id, Text = "Att öka nätverkshastigheten", IsCorrect = false },
            new AnswerOption { QuestionId = q5.Id, Text = "Att filtrera och kontrollera nätverkstrafik baserat på regler", IsCorrect = true },
            new AnswerOption { QuestionId = q5.Id, Text = "Att lagra lösenord säkert", IsCorrect = false },
            new AnswerOption { QuestionId = q5.Id, Text = "Att kryptera hårddisken", IsCorrect = false },

            // q6 - HTTP vs HTTPS
            new AnswerOption { QuestionId = q6.Id, Text = "HTTPS är snabbare än HTTP", IsCorrect = false },
            new AnswerOption { QuestionId = q6.Id, Text = "HTTPS krypterar trafiken med TLS, HTTP skickar data i klartext", IsCorrect = true },
            new AnswerOption { QuestionId = q6.Id, Text = "Det är ingen skillnad, de är utbytbara", IsCorrect = false },
            new AnswerOption { QuestionId = q6.Id, Text = "HTTP är säkrare för inloggning", IsCorrect = false },

            // q7 - DDoS
            new AnswerOption { QuestionId = q7.Id, Text = "En attack som krypterar dina filer och kräver lösen", IsCorrect = false },
            new AnswerOption { QuestionId = q7.Id, Text = "En attack som överbelastar en server med trafik för att göra den otillgänglig", IsCorrect = true },
            new AnswerOption { QuestionId = q7.Id, Text = "En metod för att stjäla lösenord via falska webbsidor", IsCorrect = false },
            new AnswerOption { QuestionId = q7.Id, Text = "En attack som utnyttjar SQL-sårbarheter", IsCorrect = false },

            // q8 - MITM
            new AnswerOption { QuestionId = q8.Id, Text = "En attack där angriparen fysiskt tar sig in i ett datacenter", IsCorrect = false },
            new AnswerOption { QuestionId = q8.Id, Text = "En attack där angriparen placerar sig mellan två parter och kan avlyssna eller manipulera kommunikationen", IsCorrect = true },
            new AnswerOption { QuestionId = q8.Id, Text = "En attack som utnyttjar svaga lösenord via automatiska försök", IsCorrect = false },
            new AnswerOption { QuestionId = q8.Id, Text = "En attack som injicerar skadlig kod i en databas", IsCorrect = false },

            // q9 - OWASP Top 10
            new AnswerOption { QuestionId = q9.Id, Text = "En lista över de 10 bästa säkerhetsverktygen", IsCorrect = false },
            new AnswerOption { QuestionId = q9.Id, Text = "En lista över de 10 vanligaste och allvarligaste webbsårbarheterna", IsCorrect = true },
            new AnswerOption { QuestionId = q9.Id, Text = "En certifiering för webbutvecklare", IsCorrect = false },
            new AnswerOption { QuestionId = q9.Id, Text = "Ett ramverk för lösenordshantering", IsCorrect = false },

            // q10 - OWASP sårbarhet
            new AnswerOption { QuestionId = q10.Id, Text = "Långsam nätverkshastighet", IsCorrect = false },
            new AnswerOption { QuestionId = q10.Id, Text = "Trasna CSS-stilar", IsCorrect = false },
            new AnswerOption { QuestionId = q10.Id, Text = "Bruten åtkomstkontroll", IsCorrect = true },
            new AnswerOption { QuestionId = q10.Id, Text = "Felaktig teckenkodning", IsCorrect = false },

            // q11 - SQL Injection exempel
            new AnswerOption { QuestionId = q11.Id, Text = "Att lägga till extra CSS-klasser i HTML", IsCorrect = false },
            new AnswerOption { QuestionId = q11.Id, Text = "Att injicera SQL via indata för att manipulera queries", IsCorrect = true },
            new AnswerOption { QuestionId = q11.Id, Text = "Att blockera portar i brandväggen", IsCorrect = false },
            new AnswerOption { QuestionId = q11.Id, Text = "Att använda HTTPS", IsCorrect = false },

            // q12 - Skydd mot SQL Injection
            new AnswerOption { QuestionId = q12.Id, Text = "Använd långa lösenord", IsCorrect = false },
            new AnswerOption { QuestionId = q12.Id, Text = "Använd parametriserade queries / prepared statements", IsCorrect = true },
            new AnswerOption { QuestionId = q12.Id, Text = "Inaktivera HTTPS", IsCorrect = false },
            new AnswerOption { QuestionId = q12.Id, Text = "Lagra lösenord i klartext", IsCorrect = false }
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