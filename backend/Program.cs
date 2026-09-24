using Checklist.Api.Data;
using Checklist.Api.Repositories;
using Checklist.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// This resolves the local database path to the current user's local application data folder.
var localDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var appDataDirectory = Path.Combine(localDataFolder, "ChecklistDesktop");
Directory.CreateDirectory(appDataDirectory);
var dbPath = Path.Combine(appDataDirectory, "checklist.db");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", corsBuilder =>
    {
        // Electron packaged builds use file:// and send Origin: null.
        corsBuilder
            .SetIsOriginAllowed(origin =>
                string.Equals(origin, "null", StringComparison.OrdinalIgnoreCase) ||
                origin.StartsWith("http://localhost:", StringComparison.OrdinalIgnoreCase) ||
                origin.StartsWith("https://localhost:", StringComparison.OrdinalIgnoreCase))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Repository registrations.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();

// Service registrations.
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IGoogleCalendarService, GoogleCalendarService>();

var app = builder.Build();

// This creates the database and default local user during startup.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(dbContext);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
