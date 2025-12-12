namespace AuctionApp.Data
{
    public class AuctionItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
    }
}