using Microsoft.EntityFrameworkCore;
using NycTaxiSearch.Data;
using NycTaxiSearch.Data.Repositories;
using NycTaxiSearch.Services;
using System.CodeDom;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.CommandTimeout(180)
    ));
// Configure Memory Cache
builder.Services.AddMemoryCache();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ITaxiTripRepository, TaxiTripRepository>();

// Register services
builder.Services.AddScoped(typeof(ICsvImportService),typeof(CsvImportService));
builder.Services.AddScoped(typeof(ITaxiSearchService), typeof(TaxiSearchService));
builder.Services.AddSingleton(typeof(ICacheService<>), typeof(CacheService<>));

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();