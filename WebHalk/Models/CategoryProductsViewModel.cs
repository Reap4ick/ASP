

using System.Collections.Generic;

namespace WebHalk.Models
{
    public class CategoryProductsViewModel
    {
        public string CategoryName { get; set; }
        public List<ProductViewModel> Products { get; set; }
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> ProductPhotos { get; set; }
    }
}
