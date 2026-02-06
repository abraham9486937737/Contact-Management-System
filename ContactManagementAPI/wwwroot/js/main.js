// Global JavaScript for Contact Management System

// Form validation
document.addEventListener('DOMContentLoaded', function() {
    initializeEventListeners();
});

function initializeEventListeners() {
    // Add click handlers for dynamic elements
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!validateForm(this)) {
                e.preventDefault();
            }
        });
    });
}

function validateForm(form) {
    // Validate required fields
    const requiredFields = form.querySelectorAll('[required]');
    let isValid = true;

    requiredFields.forEach(field => {
        if (!field.value.trim()) {
            field.classList.add('error');
            isValid = false;
        } else {
            field.classList.remove('error');
        }
    });

    // Validate email fields
    const emailFields = form.querySelectorAll('input[type="email"]');
    emailFields.forEach(field => {
        if (field.value && !isValidEmail(field.value)) {
            field.classList.add('error');
            isValid = false;
        } else {
            field.classList.remove('error');
        }
    });

    return isValid;
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Add visual feedback on input
document.addEventListener('DOMContentLoaded', function() {
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value.trim()) {
                this.classList.add('filled');
            }
        });

        input.addEventListener('focus', function() {
            this.classList.remove('error');
        });
    });
});

// Utility function to format file size
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
}

// Notification system
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.innerHTML = `
        <i class="fas fa-${type === 'error' ? 'exclamation-circle' : type === 'success' ? 'check-circle' : 'info-circle'}"></i>
        <span>${message}</span>
    `;

    document.body.appendChild(notification);

    // Auto-remove after 5 seconds
    setTimeout(() => {
        notification.classList.add('fade-out');
        setTimeout(() => notification.remove(), 300);
    }, 5000);
}

// Confirm dialog wrapper
function confirmAction(message) {
    return confirm(message);
}

// Export contact details
function exportContact(contactId) {
    // This can be extended to export to PDF, Excel, vCard, etc.
    console.log('Exporting contact:', contactId);
}

// Print contact details
function printContact(contactId) {
    window.print();
}

// Session timeout warning
let sessionWarningShown = false;
const sessionTimeout = 30 * 60 * 1000; // 30 minutes

setTimeout(function() {
    if (!sessionWarningShown) {
        console.warn('Session will expire in 5 minutes');
        sessionWarningShown = true;
    }
}, sessionTimeout - 5 * 60 * 1000);

// Add loading spinner to buttons
document.addEventListener('click', function(e) {
    if (e.target.type === 'submit' && e.target.classList.contains('btn')) {
        const originalHTML = e.target.innerHTML;
        e.target.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
        e.target.disabled = true;

        // Re-enable button after 3 seconds (adjust as needed)
        setTimeout(() => {
            e.target.innerHTML = originalHTML;
            e.target.disabled = false;
        }, 3000);
    }
});
