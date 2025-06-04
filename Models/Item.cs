using System.ComponentModel.DataAnnotations;

namespace Melotrade.Models
{
    public class Item
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Category { get; set; } // e.g. Laptop, Monitor
        public string Description { get; set; }
        public string active { get; set; }
        public string ItemNr { get; set; }
        public decimal Price { get; set; }
      
        public bool IsSold { get; set; } = false;

        public string? FormId { get; set; } = Guid.NewGuid().ToString();

        public List<ItemImage> ItemImages { get; set; } = new List<ItemImage>();
    }
}
