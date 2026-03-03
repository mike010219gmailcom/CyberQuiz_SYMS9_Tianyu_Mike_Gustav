using CyberQuiz.DAL;
using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Seeding;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Controllers / API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DAL (DbContext + repositories)
builder.Services.AddDal(builder.Configuration);

// Identity (behövs för default-user seed)
builder.Services
    .AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<CyberQuizDbContext>()
    .AddDefaultTokenProviders();

// Auth (cookies är enklast om UI+API kör samma host, annars JWT)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/access-denied";
});

var app = builder.Build();

// Seed + migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CyberQuizDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    await DatabaseSeeder.SeedAsync(db, userManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();