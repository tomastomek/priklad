using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zufanci.Shared
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Abbreviation { get; set; }
        [Required]
        public int Value { get; set; }
        [Required]
        public int DisplayValue { get; set; }
    }
}
