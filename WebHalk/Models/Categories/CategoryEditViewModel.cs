using System.ComponentModel.DataAnnotations;

namespace WebHalk.Models.Categories
{
    public class CategoryEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
