# Josh's Auction

A real-time auction app for running auctions with friends.

Built with ASP.NET Core, SignalR for live bid updates, and SQL Server for persistence.

## Features

- **Live bidding** - Prices update in real-time across all connected browsers via SignalR
- **Role-based access** - Bidders see the auction room, admins see a live bid monitor
- **Session auth** - Simple login system, nothing fancy

## Tech Stack

- ASP.NET Core (.NET 10) with MVC + Razor views
- SignalR for WebSocket-based real-time updates
- SQL Server (LocalDB for dev)
- Bootstrap 5 + custom CSS

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (comes with Visual Studio) or a SQL Server instance

### Setup

1. **Clone the repo**

   ```bash
   git clone https://github.com/your-username/AuctionApp.git
   cd AuctionApp
   ```

2. **Set up the database**

   Run `Database/Setup.sql` against your SQL Server instance. This creates the `AuctionDB` database, tables, stored procedures, and seed data.

3. **Update the connection string** (if needed)

   The default in `appsettings.json` points to LocalDB:

   ```
   Server=(localdb)\MSSQLLocalDB;Database=AuctionDB;Integrated Security=true;TrustServerCertificate=true;
   ```

4. **Run it**

   ```bash
   dotnet run
   ```

   Open [http://localhost:5000](http://localhost:5000) in your browser.

### Test Accounts

| Username | Password   | Role   |
|----------|------------|--------|
| user1    | password   | Bidder |
| user2    | password   | Bidder |
| admin    | password   | Admin  |

## Project Structure

```
Controllers/     MVC controllers (Home, Bidder, Admin)
Data/            Data access layer + models (AuctionDLL, Bidder, AuctionItem, Bid)
Hubs/            SignalR hub for real-time bid broadcasting
Views/           Razor views for each page
Database/        SQL setup script with schema + seed data
wwwroot/         Static assets (CSS, JS)
```

## How It Works

1. Users log in and get routed to either the **Auction Room** (bidders) or the **Bid Monitor** (admins)
2. Bidders place bids on items - bids must exceed the current price
3. When a bid is placed, SignalR broadcasts the update to all connected clients
4. Admins see a live feed of all bid activity
