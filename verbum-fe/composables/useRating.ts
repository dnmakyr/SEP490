import { useToast } from '~/components/ui/toast'

export interface Rating {
  ratingId?: string
  orderId: string
  inTime: number
  expectation: number
  issueResolved: number
  moreThought: string
}

const { toast } = useToast()

export const useRating = () => {
  const ratings = ref<Rating[]>([])
  const filteredRating = ref<Rating | null>(null)
  const isLoading = ref(false)

  const getRatings = async () => {
    isLoading.value = true
    try {
      const { data: ratingData } = await useAPI<Rating[]>('/rating/get-all', {
        method: 'GET'
      })

      if (!ratingData?.value || ratingData.value.length === 0) {
        // toast({
        //   title: 'No ratings found!',
        //   description: 'There are no ratings available!!'
        // })
        ratings.value = []
      } else {
        ratings.value = ratingData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching ratings!!',
        description: 'An error occurred while fetching ratings!!'
      })
      console.log('Error fetching ratings: ', error)
    } finally {
      isLoading.value = false
    }
  }

  const getRatingByOrderId = async (orderId: string) => {
    try {
      if (!filteredRating.value) {
        await getRatings()
        filteredRating.value = ratings.value.find(rating => rating.orderId === orderId) || null
        
      }
      
    } catch (error) {
      console.error('Error fetching rating by orderId:', error)
    }
  }

  const createRating = async (rating: Rating) => {
    try {
      await useAPI('/rating/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(rating)
      })
      toast({
        title: 'Order Rated Successfully!!!',
        description: `Thank you for rating your order! Your feedback helps us improve our service.`
      })
    } catch (error) {
      toast({
        title: 'Error creating rating',
        description: 'An error occurred while creating the rating.'
      })
      console.error('Error creating rating:', error)
    }
  }

  const updateRating = async (rating: Rating) => {
    try {
      const payload = rating
      await useAPI('/rating/update', {
        method: 'PUT',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
      })

      toast({
        title: 'Your rating updated !!',
        description: `Your rating has been updated!!`
      })
    } catch (error) {
      toast({
        title: 'Error updating your rating',
        description: 'An error occurred while updating the your rating!!'
      })
      console.error('Error updating rating:', error)
    }
  }

  const deleteRating = async (id: string) => {
    try {
      await useAPI(`/rating/delete`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        params: {guid: id}
      })
      toast({
        title: 'Rating deleted',
        description: `Rating has been deleted`
      })
    } catch (error) {
      toast({
        title: 'Error deleting your rating',
        description: 'An error occurred while deleting the your rating.'
      })
      console.error('Error deleting your rating:', error)
    }
  }

  return {
    ratings,
    isLoading,
    filteredRating,
    getRatings,
    getRatingByOrderId,
    createRating,
    updateRating,
    deleteRating
  }
}
