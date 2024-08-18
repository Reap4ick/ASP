using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebHalk.Data;
using WebHalk.Models.Products;
using System.Threading.Tasks;

namespace WebHalk.Controllers
{
    public class ProductController : Controller
    {
        private readonly HulkDbContext _context;

        public ProductController(HulkDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.ProductPhotos)
                .ToListAsync();

            return View(products);
        }
    }
}
