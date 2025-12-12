-- Simple Auction Database Setup
USE master;
GO

-- Drop and recreate database
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'AuctionDB')
    DROP DATABASE AuctionDB;
GO

CREATE DATABASE AuctionDB;
GO

USE AuctionDB;
GO

-- Create Tables
CREATE TABLE Item (
    ItemId INT PRIMARY KEY IDENTITY(1,1),
    ItemName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(MAX),
    CurrentPrice FLOAT DEFAULT 0.0
);

CREATE TABLE Bidder (
    BidderID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(256) NOT NULL UNIQUE,
    Password NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Bidder'
);

CREATE TABLE Bid (
    BidID INT PRIMARY KEY IDENTITY(1,1),
    ItemID INT NOT NULL,
    BidderID INT NOT NULL,
    Amount FLOAT NOT NULL,
    BidTime DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ItemID) REFERENCES Item(ItemId),
    FOREIGN KEY (BidderID) REFERENCES Bidder(BidderID)
);

-- Insert Test Data
INSERT INTO Bidder (Username, Password, Role) VALUES 
    ('user1', 'password', 'Bidder'),
    ('user2', 'password', 'Bidder'),
    ('admin', 'password', 'Admin');

INSERT INTO Item (ItemName, Description, CurrentPrice) VALUES 
    ('Vintage Watch', 'Beautiful antique watch from 1920s', 150.00),
    ('Painting', 'Original oil painting by a local artist', 500.00),
    ('Bicycle', 'Classic road bike in excellent condition', 200.00);
GO

-- Create Stored Procedures
CREATE PROCEDURE LoginUser
    @Username NVARCHAR(256),
    @Password NVARCHAR(256)
AS
BEGIN
    SELECT BidderID, Username, Role
    FROM Bidder
    WHERE Username = @Username AND Password = @Password;
END
GO

CREATE PROCEDURE GetAuction
AS
BEGIN
    SELECT ItemId, ItemName, Description, CurrentPrice
    FROM Item;
END
GO

CREATE PROCEDURE PlaceBid
    @ItemID INT,
    @BidderID INT,
    @Amount FLOAT
AS
BEGIN
    DECLARE @CurrentPrice FLOAT;
    SELECT @CurrentPrice = CurrentPrice FROM Item WHERE ItemId = @ItemID;

    IF @Amount > @CurrentPrice
    BEGIN
        INSERT INTO Bid (ItemID, BidderID, Amount, BidTime)
        VALUES (@ItemID, @BidderID, @Amount, GETDATE());
        
        UPDATE Item SET CurrentPrice = @Amount WHERE ItemId = @ItemID;
        SELECT 1 AS result;
    END
    ELSE
    BEGIN
        SELECT 0 AS result;
    END
END
GO

CREATE PROCEDURE GetAllBids
AS
BEGIN
    SELECT b.BidID, b.ItemID, b.BidderID, b.Amount, b.BidTime,
           i.ItemName, br.Username, br.Role
    FROM Bid b
    JOIN Item i ON b.ItemID = i.ItemId
    JOIN Bidder br ON b.BidderID = br.BidderID
    ORDER BY b.BidTime DESC;
END
GO