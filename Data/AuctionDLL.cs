using Microsoft.Data.SqlClient;
using System.Data;

namespace AuctionApp.Data
{
    public class AuctionDLL
    {
        private readonly string connectionString;

        public AuctionDLL(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public Bidder? LoginUser(string username, string password)
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("LoginUser", connection);
            
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Bidder
                {
                    BidderID = (int)reader["BidderID"],
                    Username = reader["Username"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? ""
                };
            }

            return null;
        }

        public List<AuctionItem> GetAuctionItems()
        {
            List<AuctionItem> items = new();

            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("GetAuction", connection);
            
            command.CommandType = CommandType.StoredProcedure;
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new AuctionItem
                {
                    ItemId = (int)reader["ItemId"],
                    ItemName = reader["ItemName"]?.ToString() ?? "",
                    Description = reader["Description"]?.ToString() ?? "",
                    CurrentPrice = Convert.ToDecimal(reader["CurrentPrice"])
                });
            }

            return items;
        }

        public bool PlaceBid(int itemId, int bidderId, decimal amount)
        {
            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("PlaceBid", connection);
            
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ItemID", itemId);
            command.Parameters.AddWithValue("@BidderID", bidderId);
            command.Parameters.AddWithValue("@Amount", amount);

            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                int result = (int)reader["result"];
                return result == 1;
            }

            return false;
        }

        public List<Bid> GetAllBids()
        {
            List<Bid> bids = new();

            using SqlConnection connection = new(connectionString);
            using SqlCommand command = new("GetAllBids", connection);
            
            command.CommandType = CommandType.StoredProcedure;
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                bids.Add(new Bid
                {
                    BidID = (int)reader["BidID"],
                    ItemID = (int)reader["ItemID"],
                    BidderID = (int)reader["BidderID"],
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    BidTime = (DateTime)reader["BidTime"],
                    ItemName = reader["ItemName"]?.ToString() ?? "",
                    Username = reader["Username"]?.ToString() ?? "",
                    Role = reader["Role"]?.ToString() ?? ""
                });
            }

            return bids;
        }
    }
}