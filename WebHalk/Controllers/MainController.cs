using Microsoft.AspNetCore.Mvc;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Models.Categories;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace WebHalk.Controllers
{
    public class MainController : Controller
    {
        private readonly HulkDbContext _hulkDbContext;
        private readonly IWebHostEnvironment _env;

        public MainController(HulkDbContext hulkDbContext, IWebHostEnvironment env)
        {
            _hulkDbContext = hulkDbContext;
            _env = env;
        }

        public IActionResult Index()
        {
            var list = _hulkDbContext.Categories
                .Select(x => new CategoryItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Image = x.Image
                })
                .ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CategoryCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string imageUrl = string.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, model.ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }
                imageUrl = $"/uploads/{model.ImageFile.FileName}";
            }
            else if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                imageUrl = model.ImageUrl;
            }

            var entity = new CategoryEntity
            {
                Image = imageUrl,
                Name = model.Name,
            };
            _hulkDbContext.Categories.Add(entity);
            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entity = _hulkDbContext.Categories.Find(id);
            if (entity == null)
            {
                return NotFound();
            }

            var model = new CategoryEditViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageUrl = entity.Image
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(CategoryEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = _hulkDbContext.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Видаляємо старе зображення, якщо завантажено нове
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var currentImagePath = Path.Combine(_env.WebRootPath, entity.Image.TrimStart('/'));
                if (System.IO.File.Exists(currentImagePath))
                {
                    System.IO.File.Delete(currentImagePath);
                }

                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                var filePath = Path.Combine(uploads, model.ImageFile.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }
                entity.Image = $"/uploads/{model.ImageFile.FileName}";
            }
            else if (!string.IsNullOrEmpty(model.ImageUrl) && model.ImageUrl != entity.Image)
            {
                var currentImagePath = Path.Combine(_env.WebRootPath, entity.Image.TrimStart('/'));
                if (System.IO.File.Exists(currentImagePath))
                {
                    System.IO.File.Delete(currentImagePath);
                }

                entity.Image = model.ImageUrl;
            }

            entity.Name = model.Name;

            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(CategoryDeleteViewModel model)
        {
            var entity = _hulkDbContext.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            // Видаляємо файл зображення
            var imagePath = Path.Combine(_env.WebRootPath, entity.Image.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _hulkDbContext.Categories.Remove(entity);
            _hulkDbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CategoryProducts(int id)
        {
            var category = await _hulkDbContext.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductPhotos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            // Перевіряємо, чи дійсно завантажуються продукти
            if (category.Products == null || !category.Products.Any())
            {
                // Лог або точка зупину для відладки
                Console.WriteLine("No products found for this category.");
            }

            return View(category);
        }


    }
}
