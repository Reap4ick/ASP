using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebHalk.Data.Entities;

namespace WebHalk.Models.Products
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public CategoryEntity Category { get; set; } = null!;

        public ICollection<ProductPhotoEntity> ProductPhotos { get; set; } = new List<ProductPhotoEntity>();
    }
}
