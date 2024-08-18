using System.Collections.Generic;

namespace WebHalk.Models.Categories
{
    public class CategoryProductsViewModel
    {
        public string CategoryName { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }

    public class ProductViewModel
    {
        public string Name { get; set; }
        public List<string> Photos { get; set; }
    }
}
