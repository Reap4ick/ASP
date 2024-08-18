using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebHalk.Models.Products;


namespace WebHalk.Data.Entities
{
    [Table("tbl_categories")]
    public class CategoryEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(255)]
        public string Image { get; set; } = string.Empty;

        // Додаємо властивість навігації до продуктів
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}
