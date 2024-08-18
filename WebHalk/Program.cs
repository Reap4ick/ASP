using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebHalk.Data;
using WebHalk.Services;

var builder = WebApplication.CreateBuilder(args);

// ������������ DbContext ��� ������������ PostgreSQL
builder.Services.AddDbContext<HulkDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ������ ������ �� ����������
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DataSeeder>(); // �������� DataSeeder

var app = builder.Build();

// ������������ ������� ������� HTTP-������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

// ��������� ��������� ��� ���������� ���������, ���� ���� �� ����
string dirSave = Path.Combine(Directory.GetCurrentDirectory(), "images");
if (!Directory.Exists(dirSave))
{
    Directory.CreateDirectory(dirSave);
}

// ������ �������� ��������� ����� � ����� "images"
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(dirSave),
    RequestPath = "/images"
});

app.UseRouting();

app.UseAuthorization();

// ������������� ��� ����������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

// �������� ������� �� ��������� ���� ��� ���������� ���� �����
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HulkDbContext>();
    dbContext.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    seeder.SeedProducts(); // ���������� ���� ����� ����������
}

app.Run();
