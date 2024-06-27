using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zufanci.Shared
{
    public class ProductPrice
    {
        public int Id { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Množství musí být větší jak 0")]
        public double Size { get; set; }
        [Range(1, double.MaxValue, ErrorMessage = "Cena musí být větší jak 0")]
        public decimal Price { get; set; }
        //public decimal UnitPrice { get; set; }
        public bool Discount { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Obchod nemůže být prázdný")]
        public int ShopId { get; set; }
        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Jednotka nemůže být prázdná")]
        public int UnitId { get; set; }
        [ForeignKey("UnitId")]
        public Unit? Unit { get; set; }
    }
}
