using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zufanci.Shared
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Jméno kategorie nemůže být prázdné.")]
        public string Name { get; set; }
        public string? CategoryImage { get; set; }
    }
}
