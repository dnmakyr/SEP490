import { useToast } from '~/components/ui/toast'

const { toast } = useToast()

export interface Languages {
  languageId: string
  languageName: string
  support: boolean
}

export const useLanguages = () => {
  const languages = ref<Languages[]>([])
  const supportedLanguages = ref([])
  const isLoading = ref(false)

  const getLanguages = async () => {
    isLoading.value = true
    try {
      const { data: languageData } = await useAPI<Languages[]>('/lang', {
        method: 'GET'
      })

      if (!languageData?.value || languageData.value.length === 0) {
        toast({
          title: 'No languages found!',
          description: 'There are no languages available!!'
        })
        languages.value = []
      } else {
        languages.value = languageData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching languages!!',
        description: 'An error occurred while fetching languages!!'
      })
      console.log('Error fetching languages: ', error)
    } finally {
      isLoading.value = false
    }
  }

  const getSupportedLanguages = async () => {
    isLoading.value = true
    try {
      const { data: languageData } = await useAPI<[]>('/lang/support', {
        method: 'GET'
      })

      if (!languageData?.value || languageData.value.length === 0) {
        toast({
          title: 'No languages found!',
          description: 'There are no languages available!!'
        })
        supportedLanguages.value = []
      } else {
        supportedLanguages.value = languageData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching languages!!',
        description: 'An error occurred while fetching languages!!'
      })
      console.log('Error fetching languages: ', error)
    } finally {
      isLoading.value = false
    }
  }

  const updateSupportedLanguages = async (
    updatedItem: {
      languageId: string
      support: boolean
    }[]
  ) => {
    try {
      const payload = updatedItem
      await useAPI('/lang/support', {
        method: 'PUT',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
      })

      toast({
        title: 'Supported languages updated !!',
        description: `Supported languages has been updated!!`
      })
    } catch (error) {
      toast({
        title: 'Error updating supported languages',
        description:
          'An error occurred while updating the supported languages!!'
      })
      console.error('Error updating supported languages:', error)
    }
  }

  return {
    isLoading,
    languages,
    supportedLanguages,
    getLanguages,
    getSupportedLanguages,
    updateSupportedLanguages
  }
}
