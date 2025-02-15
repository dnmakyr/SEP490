

export default defineNuxtPlugin(() => {
    if (import.meta.client) {  // Replaced process.client with import.meta.client
      const script = document.createElement('script');
      script.src = `https://www.paypal.com/sdk/js?client-id=YOUR_CLIENT_ID&currency=USD`;
      script.async = true;
      document.head.appendChild(script);
    }
  });
  
  