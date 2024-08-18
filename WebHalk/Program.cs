using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebHalk.Data;
using WebHalk.Services;

var builder = WebApplication.CreateBuilder(args);

// Налаштування DbContext для використання PostgreSQL
builder.Services.AddDbContext<HulkDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Додаємо сервіси до контейнера
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DataSeeder>(); // Реєструємо DataSeeder

var app = builder.Build();

// Налаштування конвеєра обробки HTTP-запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

// Створюємо директорію для збереження зображень, якщо вона не існує
string dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
if (!Directory.Exists(dirSave))
{
    Directory.CreateDirectory(dirSave);
}

// Додаємо підтримку статичних файлів з папки "images"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dirSave),
    RequestPath = "/images"
});

app.UseRouting();

app.UseAuthorization();

// Маршрутизація для контролерів
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

// Виконуємо міграції та запускаємо сідер для заповнення бази даних
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HulkDbContext>();
    dbContext.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.SeedProducts(); // Заповнюємо базу даних продуктами
}

app.Run();
