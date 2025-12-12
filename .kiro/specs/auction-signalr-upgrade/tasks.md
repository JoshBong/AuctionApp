# Implementation Plan

## Overview
This plan transforms the existing auction app into a professor-compliant system with minimal complexity while meeting all requirements: SignalR real-time updates, N-Tier architecture with AuctionDLL, and Azure deployment readiness.

## Tasks

- [x] 1. Fix project configuration and framework targeting


  - Update AuctionApp.csproj to target .NET 8.0 (currently .NET 10.0)
  - Ensure Visual Studio 2022 compatibility
  - _Requirements: 8.1, 8.2_

- [x] 2. Create AuctionDLL class library project


  - Create new Class Library project targeting .NET 8.0
  - Add reference from main web project to AuctionDLL
  - _Requirements: 2.1_

- [x] 2.1 Implement data access classes in AuctionDLL with 'l' prefix naming


  - Create lAuctionManager class with stored procedure methods
  - Create lBidder, lAuctionItem, lBid entity classes with 'l' prefixes
  - Move all database access logic from controllers to AuctionDLL
  - _Requirements: 2.2, 2.3, 2.4, 2.5_

- [ ]* 2.2 Write property test for data access layer
  - **Property 4: Authentication and role-based routing**
  - **Validates: Requirements 3.2, 3.3**

- [ ]* 2.3 Write property test for session management
  - **Property 5: Session state persistence**


  - **Validates: Requirements 3.4**

- [ ] 3. Add SignalR to the web application
  - Install Microsoft.AspNetCore.SignalR NuGet package


  - Configure SignalR in Program.cs
  - _Requirements: 1.1_

- [ ] 3.1 Create AuctionHub for real-time communication
  - Implement AuctionHub class with JoinAuctionRoom and NotifyBidPlaced methods
  - Add hub endpoint configuration
  - _Requirements: 1.1, 1.4_

- [ ]* 3.2 Write property test for SignalR broadcast functionality
  - **Property 1: Real-time bid broadcast consistency**


  - **Validates: Requirements 1.1, 1.3, 1.5**

- [ ]* 3.3 Write property test for SignalR connection establishment
  - **Property 3: SignalR connection establishment**
  - **Validates: Requirements 1.4**

- [ ] 4. Update controllers to use AuctionDLL and SignalR
  - Modify HomeController to use lAuctionManager for authentication


  - Update BidderController to use AuctionDLL and broadcast via SignalR
  - Update AdminController to use AuctionDLL for bid history
  - _Requirements: 2.4, 4.4, 4.5_

- [ ]* 4.1 Write property test for bid validation workflow
  - **Property 9: Bid validation and processing workflow**
  - **Validates: Requirements 4.4, 4.5**



- [ ] 5. Add JavaScript SignalR client to views
  - Add SignalR JavaScript library reference
  - Implement client-side connection and bid update handling
  - Update Bidder/Index.cshtml with real-time price updates
  - _Requirements: 1.2, 1.3_

- [ ]* 5.1 Write property test for client-side updates
  - **Property 2: Client-side price update reactivity**
  - **Validates: Requirements 1.2**

- [ ] 6. Enhance auction room UI for real-time bidding
  - Update Bidder/Index.cshtml to show real-time price changes


  - Add visual indicators for bid status and connection state
  - Ensure all auction items display with bid forms
  - _Requirements: 4.1, 4.2, 4.3_

- [ ]* 6.1 Write property test for auction room display
  - **Property 7: Auction room data display completeness**
  - **Validates: Requirements 4.1, 4.2**

- [ ]* 6.2 Write property test for bid form availability
  - **Property 8: Bid form availability**
  - **Validates: Requirements 4.3**

- [ ] 7. Update admin dashboard for real-time monitoring
  - Modify Admin/Index.cshtml to receive real-time bid updates
  - Ensure proper bid history ordering and display



  - _Requirements: 5.1, 5.2, 5.4, 5.5_




- [ ]* 7.1 Write property test for admin bid history
  - **Property 10: Admin bid history completeness**
  - **Validates: Requirements 5.1, 5.2**




- [ ]* 7.2 Write property test for bid ordering
  - **Property 11: Bid history chronological ordering**
  - **Validates: Requirements 5.4**

- [ ]* 7.3 Write property test for real-time admin updates
  - **Property 12: Real-time admin updates**
  - **Validates: Requirements 5.5**

- [ ] 8. Checkpoint - Ensure all tests pass and real-time functionality works
  - Ensure all tests pass, ask the user if questions arise
  - Test SignalR functionality with multiple browser windows
  - Verify N-Tier architecture is properly implemented

- [ ] 9. Configure for Azure deployment
  - Update connection strings for Azure SQL Database
  - Add Azure-specific configuration settings
  - Ensure SignalR works with Azure App Service
  - _Requirements: 7.1, 7.2, 7.3_

- [ ] 10. Final verification and testing
  - Test complete user workflows (login → bid → real-time updates)
  - Verify all professor requirements are met
  - Prepare for Azure deployment demonstration
  - _Requirements: 7.5_