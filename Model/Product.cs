using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }

        [Required]
        public string Sku { get; set; }
        public bool IsActive { get; set; }
    }
}
