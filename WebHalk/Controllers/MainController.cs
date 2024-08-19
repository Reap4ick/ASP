using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using WebHalk.Models.Categories;
using WebHalk.Models.Products;

namespace WebHalk.Controllers
{
    public class MainController : Controller
    {
        private readonly HulkDbContext _context;
        private readonly ILogger<MainController> _logger;
        private readonly IWebHostEnvironment _env;

        public MainController(HulkDbContext context, ILogger<MainController> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            var list = _context.Categories
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
            _context.Categories.Add(entity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var entity = _context.Categories.Find(id);
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

            var entity = _context.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

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

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(CategoryDeleteViewModel model)
        {
            var entity = _context.Categories.Find(model.Id);
            if (entity == null)
            {
                return NotFound();
            }

            var imagePath = Path.Combine(_env.WebRootPath, entity.Image.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _context.Categories.Remove(entity);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CategoryProducts(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.ProductPhotos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products == null || !category.Products.Any())
            {
                Console.WriteLine("No products found for this category.");
            }

            return View(category);
        }

        [HttpGet]
        public IActionResult CreateProduct(int categoryId)
        {
            var model = new ProductCreateViewModel
            {
                CategoryId = categoryId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            _logger.LogInformation("CreateProduct method is called.");

            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("ModelState is valid. Proceeding with product creation.");

                    var product = new ProductEntity
                    {
                        Name = model.Name,
                        CategoryId = model.CategoryId
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product added with ID: {0}", product.Id);

                    if (model.ImageFiles != null && model.ImageFiles.Any())
                    {
                        foreach (var file in model.ImageFiles)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            _context.ProductPhotos.Add(new ProductPhotoEntity
                            {
                                ImageUrl = "/uploads/" + fileName,
                                ProductId = product.Id
                            });

                            _logger.LogInformation("Image file added: {0}", fileName);
                        }
                    }

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Product creation completed.");

                    return RedirectToAction("CategoryProducts", new { id = model.CategoryId });
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while creating the product: {0}", ex.Message);
                    _logger.LogError("Stack Trace: {0}", ex.StackTrace);
                    ModelState.AddModelError("", "An error occurred while creating the product. Please try again.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState is invalid. Product creation failed.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error: {0}", error.ErrorMessage);
                }
            }

            return View(model);
        }
    }
}
