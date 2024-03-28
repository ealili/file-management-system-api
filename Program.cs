using System.Text;
using FileSystemManagementApi.Data;
using FileSystemManagementApi.Infrastructure;
using FileSystemManagementApi.Models;
using FileSystemManagementApi.Repositories.Implementations;
using FileSystemManagementApi.Repositories.Interfaces;
using FileSystemManagementApi.Services;
using FileSystemManagementApi.Services.Implementations;
using FileSystemManagementApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
builder.Services.AddSingleton(configuration);

builder.Services.AddSingleton<FileWatcherService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    // options.UseInMemoryDatabase("InMemoryDatabase"); 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var allowedOrigin = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? throw new InvalidOperationException("Allowed Origins config is missing.");

builder.Services.AddCors(options =>
{
    options.AddPolicy("appCors", policy =>
    {
        policy.WithOrigins(allowedOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtSecret = builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException("JWT Secret config is missing.");

var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),

    ValidateIssuer = true,
    ValidIssuer = builder.Configuration["JWT:Issuer"],
    ValidateAudience = true,
    ValidAudience = builder.Configuration["JWT:Audience"],
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IJwtManagerService, JwtManagerService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();




builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("appCors");

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

// Seed Database
AppDbInitializer.SeedRolesToDb(app).Wait();

app.MapControllers();

var fileWatcherService = app.Services.GetRequiredService<FileWatcherService>();
fileWatcherService.Start();

app.Run();