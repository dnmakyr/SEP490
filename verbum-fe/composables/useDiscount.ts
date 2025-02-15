import { useToast } from '~/components/ui/toast'

export interface Discount {
  discountId: string
  discountName: string
  discountPercent: number
}

const { toast } = useToast()
export const useDiscounts = () => {
  const discounts = ref<Discount[]>([])
  const isLoading = ref(false)

  const getDiscounts = async () => {
    isLoading.value = true
    try {
      const { data: discountsData } = await useAPI<Discount[]>('/discount', {
        method: 'GET'
      })
      if (!discountsData?.value || discountsData.value.length === 0) {
        toast({
          title: 'No discounts found!',
          description: 'There are no discounts available!!'
        })
        discounts.value = []
      } else {
        discounts.value = discountsData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching discounts!!',
        description: 'An error occurred while fetching discounts!!'
      })
      console.log('Error fetching discounts: ', error)
    } finally {
      isLoading.value = false
    }
  }

  const updateDiscount = async (discount: Discount) => {
    try {
      const payload = discount
      await useAPI('/discount', {
        method: 'PUT',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
      })

      toast({
        title: 'Discount updated !!',
        description: `Discount has been updated!!`
      })
    } catch (error) {
      toast({
        title: 'Error updating discount',
        description: 'An error occurred while updating the discount!!'
      })
      console.error('Error updating discount:', error)
    }
  }

  const createDiscount = async (discount: Discount) => {
    try {
      await useAPI('/discount', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(discount)
      })
      toast({
        title: 'Discount created!!',
        description: `Discount ${discount.discountName} has been created`
      })
    } catch (error) {
      toast({
        title: 'Error adding discount',
        description: 'An error occurred while adding the discount.'
      })
      console.error('Error adding discount:', error)
    }
  }

  const deleteDiscount = async (id: string) => {
    try {
      await useAPI(`discount?discountId=${id}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' }
      })
      toast({
        title: 'Category deleted',
        description: `Category with ID ${id} has been deleted`
      })
    } catch (error) {
      toast({
        title: 'Error deleting discount',
        description: 'An error occurred while deleting the discount.'
      })
      console.error('Error deleting discount:', error)
    }
  }

  const getDiscountById = async (id: string) => {
    isLoading.value = true
    try {
      const { data: discountsData } = await useAPI<Discount>(`/discount/get-by-id?id=${id}`, {
        method: 'GET'
      })
      if (!discountsData?.value) {
        toast({
          title: 'No discounts found!',
          description: 'There are no discounts available!!'
        })
        discounts.value = []
      } else {
        toast({
          title: 'Apply discount code success!',
          description: 'Enjoy your discount!!'
        })
        return discountsData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching discounts!!',
        description: 'An error occurred while fetching discounts!!'
      })
      console.log('Error fetching discounts: ', error)
    } finally {
      isLoading.value = false
    }
  }

  return {
    discounts,
    isLoading,
    getDiscounts,
    updateDiscount,
    createDiscount,
    deleteDiscount,
    getDiscountById
  }
}
