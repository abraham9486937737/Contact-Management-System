// Global JavaScript for Contact Management System
// Enhanced for mobile devices and better user experience

// Initialize on page load
document.addEventListener('DOMContentLoaded', function() {
    initializeMobileMenu();
    initializeEventListeners();
    initializeTouchGestures();
    initializeServiceWorker();
});

/* ============================================ */
/* MOBILE MENU FUNCTIONALITY                    */
/* ============================================ */
function initializeMobileMenu() {
    const menuToggle = document.getElementById('mobileMenuToggle');
    const navbarLinks = document.getElementById('navbarLinks');
    const menuOverlay = document.getElementById('mobileMenuOverlay');
    
    if (!menuToggle || !navbarLinks || !menuOverlay) return;
    
    // Toggle menu on button click
    menuToggle.addEventListener('click', function(e) {
        e.stopPropagation();
        toggleMobileMenu();
    });
    
    // Close menu when overlay is clicked
    menuOverlay.addEventListener('click', closeMobileMenu);
    
    // Close menu on navigation link click
    const navLinks = navbarLinks.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', closeMobileMenu);
    });
    
    // Close menu on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape' && navbarLinks.classList.contains('active')) {
            closeMobileMenu();
        }
    });
    
    // Prevent scroll when menu is open
    function toggleBodyScroll(disable) {
        if (disable) {
            document.body.style.overflow = 'hidden';
        } else {
            document.body.style.overflow = '';
        }
    }
    
    function toggleMobileMenu() {
        const isActive = navbarLinks.classList.toggle('active');
        menuOverlay.classList.toggle('active');
        toggleBodyScroll(isActive);
        
        // Update aria attributes for accessibility
        menuToggle.setAttribute('aria-expanded', isActive);
        
        // Change icon
        const icon = menuToggle.querySelector('i');
        if (icon) {
            icon.className = isActive ? 'fas fa-times' : 'fas fa-bars';
        }
    }
    
    function closeMobileMenu() {
        navbarLinks.classList.remove('active');
        menuOverlay.classList.remove('active');
        toggleBodyScroll(false);
        menuToggle.setAttribute('aria-expanded', 'false');
        
        const icon = menuToggle.querySelector('i');
        if (icon) {
            icon.className = 'fas fa-bars';
        }
    }
}

/* ============================================ */
/* FORM VALIDATION AND ENHANCEMENT              */
/* ============================================ */
function initializeEventListeners() {
    // Add click handlers for dynamic elements
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!validateForm(this)) {
                e.preventDefault();
                return;
            }

            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                const originalHTML = submitBtn.innerHTML;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
                submitBtn.disabled = true;

                // Restore button if submission is blocked by navigation issues
                setTimeout(() => {
                    submitBtn.innerHTML = originalHTML;
                    submitBtn.disabled = false;
                }, 5000);
            }
        });
    });
    
    // Auto-dismiss alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.opacity = '0';
            alert.style.transform = 'translateY(-20px)';
            setTimeout(() => alert.remove(), 300);
        }, 5000);
    });
    
    // Add ripple effect to buttons
    addRippleEffect();
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

/* ============================================ */
/* TOUCH GESTURES AND MOBILE INTERACTIONS       */
/* ============================================ */
function initializeTouchGestures() {
    // Swipe to close mobile menu
    let touchStartX = 0;
    let touchEndX = 0;
    
    const navbarLinks = document.getElementById('navbarLinks');
    if (navbarLinks) {
        navbarLinks.addEventListener('touchstart', function(e) {
            touchStartX = e.changedTouches[0].screenX;
        }, { passive: true });
        
        navbarLinks.addEventListener('touchend', function(e) {
            touchEndX = e.changedTouches[0].screenX;
            handleSwipe();
        }, { passive: true });
    }
    
    function handleSwipe() {
        const swipeThreshold = 100;
        const diff = touchStartX - touchEndX;
        
        // Swipe right to close menu
        if (diff < -swipeThreshold && navbarLinks.classList.contains('active')) {
            const menuOverlay = document.getElementById('mobileMenuOverlay');
            navbarLinks.classList.remove('active');
            if (menuOverlay) menuOverlay.classList.remove('active');
            document.body.style.overflow = '';
            
            const menuToggle = document.getElementById('mobileMenuToggle');
            if (menuToggle) {
                menuToggle.setAttribute('aria-expanded', 'false');
                const icon = menuToggle.querySelector('i');
                if (icon) icon.className = 'fas fa-bars';
            }
        }
    }
    
    // Add pull-to-refresh for contact list
    if (window.location.pathname === '/' || window.location.pathname.includes('home/index')) {
        initializePullToRefresh();
    }
    
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

function initializePullToRefresh() {
    let touchStartY = 0;
    let touchMoveY = 0;
    let pullDistance = 0;
    const pullThreshold = 80;
    
    document.addEventListener('touchstart', function(e) {
        if (window.scrollY === 0) {
            touchStartY = e.touches[0].clientY;
        }
    }, { passive: true });
    
    document.addEventListener('touchmove', function(e) {
        if (window.scrollY === 0) {
            touchMoveY = e.touches[0].clientY;
            pullDistance = touchMoveY - touchStartY;
        }
    }, { passive: true });
    
    document.addEventListener('touchend', function() {
        if (pullDistance > pullThreshold) {
            location.reload();
        }
        pullDistance = 0;
    }, { passive: true });
}

/* ============================================ */
/* RIPPLE EFFECT FOR BUTTONS                    */
/* ============================================ */
function addRippleEffect() {
    const buttons = document.querySelectorAll('.btn, .nav-link');
    
    buttons.forEach(button => {
        button.addEventListener('click', function(e) {
            const ripple = document.createElement('span');
            ripple.classList.add('ripple');
            this.appendChild(ripple);
            
            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';
            
            setTimeout(() => ripple.remove(), 600);
        });
    });
}

/* ============================================ */
/* SERVICE WORKER FOR PWA SUPPORT               */
/* ============================================ */
function initializeServiceWorker() {
    if ('serviceWorker' in navigator) {
        // Unregister any existing service workers to avoid serving stale/offline cached assets
        navigator.serviceWorker.getRegistrations().then(registrations => {
            registrations.forEach(reg => {
                reg.unregister().then(() => console.log('Service Worker unregistered:', reg.scope)).catch(() => {});
            });
        }).catch(() => {});

        // Do NOT register a new service worker in the dev environment to avoid cache issues
        console.log('Service Worker: unregistered existing workers and skipping registration in dev.');
    }
}

/* ============================================ */
/* NETWORK STATUS DETECTION                     */
/* ============================================ */
function initializeNetworkStatus() {
    function updateOnlineStatus() {
        if (navigator.onLine) {
            console.log('Back online');
            const offlineMessage = document.querySelector('.offline-message');
            if (offlineMessage) offlineMessage.remove();
        } else {
            console.log('Connection lost');
            const message = document.createElement('div');
            message.className = 'offline-message';
            message.innerHTML = '<i class="fas fa-wifi"></i> No internet connection';
            message.style.cssText = 'position: fixed; top: 60px; left: 0; right: 0; background: #f44336; color: white; padding: 10px; text-align: center; z-index: 9999;';
            document.body.appendChild(message);
        }
    }
    
    window.addEventListener('online', updateOnlineStatus);
    window.addEventListener('offline', updateOnlineStatus);
}

initializeNetworkStatus();

/* ============================================ */
/* LAZY LOADING FOR IMAGES                      */
/* ============================================ */
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                if (img.dataset.src) {
                    img.src = img.dataset.src;
                    img.removeAttribute('data-src');
                    imageObserver.unobserve(img);
                }
            }
        });
    });
    
    document.querySelectorAll('img[data-src]').forEach(img => {
        imageObserver.observe(img);
    });
}
