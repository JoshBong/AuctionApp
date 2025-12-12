// SignalR connection for real-time auction updates
let connection = null;

function initializeSignalR() {
    if (typeof signalR === 'undefined') {
        console.log('SignalR not loaded yet, retrying...');
        setTimeout(initializeSignalR, 100);
        return;
    }

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/auctionHub")
        .build();

    // Handle bid updates
    connection.on("BidUpdated", function (itemId, newPrice, bidderName) {
        updateItemPrice(itemId, newPrice, bidderName);
        showNotification(`New bid of $${newPrice} placed by ${bidderName}!`);
    });

    // Handle user join/leave notifications
    connection.on("UserJoined", function (username) {
        console.log(`${username} joined the auction`);
    });

    connection.on("UserLeft", function (username) {
        console.log(`${username} left the auction`);
    });

    // Start the connection
    connection.start().then(function () {
        console.log("SignalR Connected");
        showConnectionStatus(true);
    }).catch(function (err) {
        console.error("SignalR Connection Error: ", err);
        showConnectionStatus(false);
    });

    // Handle connection closed
    connection.onclose(function () {
        console.log("SignalR Disconnected");
        showConnectionStatus(false);
        // Attempt to reconnect after 5 seconds
        setTimeout(function () {
            connection.start();
        }, 5000);
    });
}

function updateItemPrice(itemId, newPrice, bidderName) {
    // Update the price display for the specific item
    const priceElement = document.querySelector(`[data-item-id="${itemId}"] .current-price`);
    if (priceElement) {
        priceElement.textContent = `$${newPrice}`;
        priceElement.classList.add('price-updated');
        setTimeout(() => {
            priceElement.classList.remove('price-updated');
        }, 2000);
    }
}

function showNotification(message) {
    // Create and show a notification
    const notification = document.createElement('div');
    notification.className = 'alert alert-info alert-dismissible fade show position-fixed';
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 1050; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

function showConnectionStatus(connected) {
    let statusElement = document.getElementById('connection-status');
    if (!statusElement) {
        statusElement = document.createElement('div');
        statusElement.id = 'connection-status';
        statusElement.className = 'position-fixed';
        statusElement.style.cssText = 'top: 10px; left: 10px; z-index: 1050; padding: 5px 10px; border-radius: 3px; font-size: 12px;';
        document.body.appendChild(statusElement);
    }
    
    if (connected) {
        statusElement.textContent = '🟢 Live Updates Active';
        statusElement.className = statusElement.className.replace(/bg-\w+/, '') + ' bg-success text-white';
    } else {
        statusElement.textContent = '🔴 Reconnecting...';
        statusElement.className = statusElement.className.replace(/bg-\w+/, '') + ' bg-danger text-white';
    }
}

// Initialize SignalR when the page loads
document.addEventListener('DOMContentLoaded', function() {
    initializeSignalR();
});
