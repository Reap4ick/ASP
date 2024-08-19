using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebHalk.Data;
using WebHalk.Data.Entities;
using WebHalk.Models.Products;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebHalk.Models;

namespace WebHalk.Controllers
{
    public class ProductController : Controller
    {
        private readonly HulkDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(HulkDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> CategoryProducts(int id)
        {
            var products = await _context.Products
                .Include(p => p.ProductPhotos)
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return View(products);
        }
    }
}
