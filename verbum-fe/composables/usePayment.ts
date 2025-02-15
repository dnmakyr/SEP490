import { useToast } from '~/components/ui/toast'

const { toast } = useToast()

export const usePayment = () => {

    const payWithPayPal = async (orderId : string, isDeposit : boolean) => {
        try {
            await useAPI('/order/payment', {
              method: 'GET',
              headers: { 'Content-Type': 'application/json' },
              params: {orderId: orderId, isDeposit: isDeposit}
            })
          } catch (error) {
            toast({
              title: 'Error paying',
              description: 'An error occurred while paying the order.'
            })
            console.error('Error paying the order:', error)
          }
      }

      return {
        payWithPayPal
      }

}