using CyberQuiz.DAL;
using CyberQuiz.DAL.Data;
using CyberQuiz.DAL.Seeding;
using CyberQuiz_BLL.Interfaces;
using CyberQuiz_BLL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers / API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - Allow UI to call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorUI", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7063", // UI port (från din terminal)
            "http://localhost:5116"   // HTTP fallback
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

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

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "CyberQuiz-Super-Secret-Key-Min-32-Chars-Long!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "CyberQuizAPI",
        ValidAudience = jwtSettings["Audience"] ?? "CyberQuizUI",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Seed + migrations (temporarily disabled until DAL fixes migrations)

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

app.UseCors("AllowBlazorUI"); // Enable CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();