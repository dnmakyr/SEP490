import { useToast } from '~/components/ui/toast'
import type { Receipt } from '~/types/receipts';


const { toast } = useToast()

export const useReceipt = () => {
  const receipts = ref<Receipt[]>([])
  const isLoading = ref(false)

  const getReceipts = async () => {
    isLoading.value = true
    try {
      const { data: receiptData } = await useAPI<Receipt[]>(
        '/receipt/get-all',
        {
          method: 'GET'
        }
      )
      if (!receiptData?.value || receiptData.value.length === 0) {
        toast({
          title: 'No receipts found!',
          description: 'There are no receipts available!!'
        })
        receipts.value = []
      } else {
        receipts.value = receiptData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching receipts!!',
        description: 'An error occurred while fetching receipts!!'
      })
      console.log('Error fetching receipts: ', error)
    } finally {
      isLoading.value = false
    }
  }
                              


  return {
    receipts,
    isLoading,
    getReceipts,
  }
}
