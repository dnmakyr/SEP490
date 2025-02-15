<!-- <script lang="ts" setup>
import { onMounted, ref } from 'vue';
import { loadScript } from '@paypal/paypal-js';
import type { PayPalScriptOptions, OnApproveActions, OnApproveData, PayPalNamespace } from '@paypal/paypal-js';
import { useToast } from '../ui/toast';
const paypalButton = ref<HTMLDivElement | null>(null);
const { successPayment } = useOrders();
const { createReceipt } = useReceipt();

const props = defineProps<{
    orderId: string,
  price: string,
  status: string
}>()
const emit = defineEmits(['payment-success']);

const depositOrPayment = props.status === 'IN_PROGRESS' ? false : true

const {toast} = useToast();
onMounted(async () => {
  if (paypalButton.value) {
    const options: PayPalScriptOptions = {
      "clientId": "AfA7cxuOdrPU6GvcxtuYqCNYrO_k2EcgtZhsI5Kl3Z_0r3_0lIHy1JymnEYuR31tGFqr2-lEIulHsTRL",  // Replace with your actual client ID
      currency: "USD"
    };

    const paypal: PayPalNamespace | null = await loadScript(options);

    if (paypal && paypal.Buttons) {
      paypal.Buttons({
        createOrder: (data, actions) => {
          return actions.order.create({
            intent: "CAPTURE",  // Specify the intent for the order
            purchase_units: [{
              amount: {
                currency_code: "USD",
                value: props.price
              }
            }]
          });
        },
        onApprove: async(data: OnApproveData, actions: OnApproveActions) => {
          if (!actions.order) {
            console.error('Order is undefined');
            return Promise.reject('Order is undefined');
          }

          await updateSuccessPayment(props.orderId, props.status)
          
          await createReceipt(props.orderId, depositOrPayment, Number(props.price))
          
          return actions.order.capture().then((details) => {
            if (details?.payer?.name?.given_name) {
              toast({
                title: `Transaction completed by ${details.payer.name.given_name}`
              })
            } else {
              toast({
                title: "Transaction completed."
              })
            }
            emit('payment-success');
          });
        },
        onError: (err) => {
          console.error('PayPal Error:', err);
        }
      }).render(paypalButton.value);
    } else {
      console.error("PayPal script failed to load.");
    }
  }
});
</script>

<template>
  <div ref="paypalButton"/>
</template>

<style scoped>
/* Add any custom styling if needed */
</style> -->
