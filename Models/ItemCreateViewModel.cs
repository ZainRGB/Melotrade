namespace Melotrade.Models
{
    public class ItemCreateViewModel
    {
        public Melotrade.Models.Item Item { get; set; }
        public IEnumerable<Melotrade.Models.Item> AvailableItems { get; set; }
        public List<Melotrade.Models.ItemImage> ExistingImages { get; set; }
    }
}
