// wwwroot/js/site.js

// Make sure the blazorGui object exists on the window, or create it.
// This helps organize multiple utility functions if you add more later.
window.blazorGui = window.blazorGui || {};

window.blazorGui.clipboardCopy = {
    copyText: function (text) {
        if (!navigator.clipboard) {
            // Clipboard API not available (e.g., older browser, insecure context)
            console.error("Clipboard API not available.");
            // Fallback for older browsers (less reliable, can be intrusive)
            try {
                const textArea = document.createElement("textarea");
                textArea.value = text;
                
                // Avoid scrolling to bottom
                textArea.style.top = "0";
                textArea.style.left = "0";
                textArea.style.position = "fixed";

                document.body.appendChild(textArea);
                textArea.focus();
                textArea.select();

                const successful = document.execCommand('copy');
                const msg = successful ? 'successful' : 'unsuccessful';
                console.log('Fallback: Copying text command was ' + msg);
                document.body.removeChild(textArea);
                return Promise.resolve(successful); // Wrap in promise to match navigator.clipboard API
            } catch (err) {
                console.error('Fallback: Oops, unable to copy', err);
                return Promise.reject(false);
            }
        }
        
        // Modern Clipboard API (requires HTTPS or localhost for security reasons)
        return navigator.clipboard.writeText(text)
            .then(() => {
                console.log('Text copied to clipboard via modern API');
                return true;
            })
            .catch(err => {
                console.error('Failed to copy text via modern API: ', err);
                return false;
            });
    }
};

// You can add other site-wide JavaScript functions here if needed,
// either inside the 'blazorGui' object or as separate window functions.