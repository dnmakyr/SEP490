// import type {User} from '~/types/user'
import { useToast } from '~/components/ui/toast'
const { toast } = useToast()

export interface User {
  id: string
  email: string
  name: string
  roleCode: string
  status: string
}

export const useUsers = () => {
  const assignList = ref<User[]>([])
  const isLoading = ref(false)
  const getAssignList = async () => {
    try {
      const { data: assignListData } = await useAPI<User[]>(
        '/user/assign-list',
        {
          method: 'GET'
        }
      )
      if (!assignListData?.value || assignListData.value.length === 0) {
        toast({
          title: 'No assignList found!',
          description: 'There are no assignList available!!'
        })
        assignList.value = []
      } else {
        assignList.value = assignListData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching assignList!!',
        description: 'An error occurred while fetching assignList!!'
      })
      console.log('Error fetching assignList: ', error)
    } finally {
      isLoading.value = false
    }
  }

  return {
    assignList,
    getAssignList
  }
}
