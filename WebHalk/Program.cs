using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebHalk.Data;
using WebHalk.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HulkDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DataSeeder>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

string dirSave = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
if (!Directory.Exists(dirSave))
{
    Directory.CreateDirectory(dirSave);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dirSave),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HulkDbContext>();
    dbContext.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.SeedProducts(); 
}

app.Run();
