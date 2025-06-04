using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Melotrade.Models
{
    public class ItemImage
    {
        [Key]
        public int Id { get; set; }

        public string FormId { get; set; } // foreign key link to Item
        public string ImageUrl { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public Item Item { get; set; }
    }
}
