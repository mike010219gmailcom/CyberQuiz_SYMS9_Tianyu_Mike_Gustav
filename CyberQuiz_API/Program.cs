using CyberQuiz.DAL;
using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Seeding;
using CyberQuiz_BLL.Interfaces;
using CyberQuiz_BLL.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Controllers / API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DAL (DbContext + repositories)
builder.Services.AddDal(builder.Configuration);

// BLL Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IUserProgressService, UserProgressService>();

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

// Swagger configurable via appsettings
var swaggerConfig = builder.Configuration.GetSection("Swagger");
var swaggerEnabled = swaggerConfig.GetValue<bool>("Enabled");
var swaggerRoutePrefix = swaggerConfig.GetValue<string>("RoutePrefix") ?? "swagger";
var swaggerEndpoint = swaggerConfig.GetValue<string>("EndpointUrl") ?? "/swagger/v1/swagger.json";
var swaggerName = swaggerConfig.GetValue<string>("Name") ?? "API";

if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = swaggerRoutePrefix;
        c.SwaggerEndpoint(swaggerEndpoint, swaggerName);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();