using DAL;
using Microsoft.EntityFrameworkCore;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Register DbContext
var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "hyperconnectx.db");
var fullPath = Path.GetFullPath(dbPath);
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite($"Data Source={fullPath}"));

// Register repositories
builder.Services.AddScoped<IGameRepository, EfGameRepository>();
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();

// Register services
builder.Services.AddScoped<BLL.PlayerStatisticsService>();

// Add session support for game state management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Initialize AI profiles
using (var scope = app.Services.CreateScope())
{
    var playerRepository = scope.ServiceProvider.GetRequiredService<IPlayerRepository>();
    AiProfileService.InitializeAiProfiles(playerRepository);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapHub<GameHub>("/gameHub");

app.Run();
