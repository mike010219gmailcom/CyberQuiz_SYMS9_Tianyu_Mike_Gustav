# CyberQuiz - Database Setup & Seeding
# info kring databasen

## Quick Start

After cloning the repository or pulling new changes with updated questions, run:

```powershell
cd C:\Users\[YourUsername]\source\repos\CyberQuiz_UI

# Drop existing database
dotnet ef database drop --project .\CyberQuiz_DAL\CyberQuiz_DAL.csproj --startup-project .\CyberQuiz_API\CyberQuiz_API.csproj --force

# Recreate database with all migrations
dotnet ef database update --project .\CyberQuiz_DAL\CyberQuiz_DAL.csproj --startup-project .\CyberQuiz_API\CyberQuiz_API.csproj
```

Then start the API project (F5 or `dotnet run`) to seed all data.

---

## Why Reset the Database?

**Database files (.mdf/.ldf) are NOT version controlled** because:
- They are binary files
- They contain local development data
- Each developer should have their own local database

**When to reset:**
- ✅ New questions added to `DatabaseSeeder.cs`
- ✅ New migrations created by DAL team
- ✅ Database schema changes
- ✅ After merging branches with database changes

---

## What Gets Seeded?

The `DatabaseSeeder.cs` currently seeds:
- **3 Categories:** Grundläggande säkerhet, Nätverk & attacker, Webbsäkerhet
- **6 SubCategories:** 2 per category
- **12 Questions:** 2 per subcategory
- **48 Answer Options:** 4 per question
- **1 Default user:** `user` / `Password1234!`

---

## Current Limitation

**DatabaseSeeder only seeds if database is empty:**
```csharp
if (await db.Categories.AnyAsync(ct))
    return; // Stops if data exists
```

This means **new questions won't be added** to an existing database automatically.

**Solution:** Drop and recreate database when `DatabaseSeeder.cs` is updated.

---

## Useful Commands

### Check database status
```powershell
dotnet ef dbcontext info --project .\CyberQuiz_DAL\ --startup-project .\CyberQuiz_API\
```

### List migrations
```powershell
dotnet ef migrations list --project .\CyberQuiz_DAL\ --startup-project .\CyberQuiz_API\
```

### Create new migration (after model changes)
```powershell
dotnet ef migrations add MigrationName --project .\CyberQuiz_DAL\ --startup-project .\CyberQuiz_API\
```

---

