namespace AuctionApp.Data
{
    public class Bid
    {
        public int BidID { get; set; }
        public int ItemID { get; set; }
        public int BidderID { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}