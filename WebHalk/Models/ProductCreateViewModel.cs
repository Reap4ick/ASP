using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebHalk.Models
{
    public class ProductCreateViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public IList<IFormFile> ImageFiles { get; set; }
    }
}
