using WebHalk.Data;
using WebHalk.Data.Entities;
using System.Linq;
using WebHalk.Models.Products;

namespace WebHalk.Services
{
    public class DataSeeder
    {
        private readonly HulkDbContext _context;

        public DataSeeder(HulkDbContext context)
        {
            _context = context;
        }

        public void SeedProducts()
        {
            if (_context.Products.Any())
            {
                return; // База даних вже заповнена
            }

            // Отримуємо категорії за їхніми ідентифікаторами
            var categoryIds = new[] { 31, 32, 33 };
            var categories = _context.Categories.Where(c => categoryIds.Contains(c.Id)).ToList();

            if (categories.Count != categoryIds.Length)
            {
                throw new Exception("Одна або кілька категорій не знайдені.");
            }

            var laptopsCategory = categories.First(c => c.Id == 31);
            var phonesCategory = categories.First(c => c.Id == 32);
            var televisionsCategory = categories.First(c => c.Id == 33);

            // Створюємо продукти
            var products = new ProductEntity[]
            {
        new ProductEntity { Name = "Ноутбук HP EliteBook 840 G10", CategoryId = laptopsCategory.Id },
        new ProductEntity { Name = "Ноутбук Dell Latitude 7640", CategoryId = laptopsCategory.Id },
        new ProductEntity { Name = "iPhone 13", CategoryId = phonesCategory.Id },
        new ProductEntity { Name = "Samsung Galaxy S21", CategoryId = phonesCategory.Id },
        new ProductEntity { Name = "Samsung TV 50\"", CategoryId = televisionsCategory.Id },
        new ProductEntity { Name = "LG OLED 65\"", CategoryId = televisionsCategory.Id }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges(); // Зберігаємо продукти, щоб ідентифікатори стали доступні

            // Отримуємо продукти з бази даних для прив'язки фотографій
            var savedProducts = _context.Products.ToList();

            if (savedProducts.Count != products.Length)
            {
                throw new Exception("Не вдалося зберегти всі продукти.");
            }

            // Створюємо фотографії для продуктів
            var productPhotos = new ProductPhotoEntity[]
            {
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Ноутбук HP EliteBook 840 G10").Id, ImageUrl = "https://example.com/images/hp_elitebook.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Ноутбук HP EliteBook 840 G10").Id, ImageUrl = "https://example.com/images/hp_elitebook_2.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Ноутбук Dell Latitude 7640").Id, ImageUrl = "https://example.com/images/dell_latitude.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Ноутбук Dell Latitude 7640").Id, ImageUrl = "https://example.com/images/dell_latitude_2.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "iPhone 13").Id, ImageUrl = "https://example.com/images/iphone13.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Samsung Galaxy S21").Id, ImageUrl = "https://example.com/images/samsung_galaxy.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "Samsung TV 50\"").Id, ImageUrl = "https://example.com/images/samsung_tv.jpg" },
        new ProductPhotoEntity { ProductId = savedProducts.First(p => p.Name == "LG OLED 65\"").Id, ImageUrl = "https://example.com/images/lg_oled.jpg" }
            };

            _context.ProductPhotos.AddRange(productPhotos);
            _context.SaveChanges();
        }

    }
}
