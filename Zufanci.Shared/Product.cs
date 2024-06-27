using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zufanci.Shared
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Je potřeba zadat název produktu")]
        public string Name { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Je potřeba vybrat kategorii")]
        public int CategoryId { get; set; }
        public string? ImageName { get; set; }
        public decimal? LowestPrice { get; set; }
        public decimal? HighestPrice { get; set; }
        public decimal? AveragePrice { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public ICollection<ProductPrice>? ProductPrices { get; set; }
    }
}
