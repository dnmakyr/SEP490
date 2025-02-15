import { useToast } from '~/components/ui/toast'

export interface Work {
  workId: string
  workName: string
  sourceLanguageId: string
  targetLanguageId: string[]
  translationFileUrls: string[]
  referenceFileUrls: string[]
  orderStatus: string
  dueDate: string
  isCompleted: boolean
  serviceCode: string
  translateService?: boolean
  editService?: boolean
  evaluateService?: boolean
}
const { toast } = useToast()

export const useWorks = () => {
  const works = ref<Work[]>([])
  const work = ref<Work>()
  const isLoading = ref(false)

  const getWorks = async () => {
    isLoading.value = true
    try {
      const { data: worksData } = await useAPI<Work[]>('/work/get-all', {
        method: 'GET'
      })
      if (!worksData?.value || worksData.value.length === 0) {
        toast({
          title: 'No works found!',
          description: 'There are no works available!!'
        })
        works.value = []
      } else {
        works.value = worksData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching works!!',
        description: 'An error occurred while fetching works!!'
      })
      console.log('Error fetching works: ', error)
    } finally {
      isLoading.value = false
    }
  }
  const getWorkById = async (workId: string) => {
    isLoading.value = true
    try {
      const { data: workData } = await useAPI<Record<string, Work>>(
        `/work/get-all?filter=WorkId eq ${workId}`,
        { method: 'GET' }
      )

      if (workData?.value) {
        const workArray = Object.values(workData.value) // Extract all values
        work.value = workArray[0] // Assign the first work
      } else {
        toast({
          title: 'No work found!',
          description: 'No data available for the given ID!'
        })
        work.value = undefined
      }
    } catch (error) {
      toast({
        title: 'Error fetching work!',
        description: 'An error occurred!'
      })
      console.error('Error:', error)
    } finally {
      isLoading.value = false
    }
  }

  return {
    works,
    work,
    isLoading,
    getWorks,
    getWorkById
  }
}
