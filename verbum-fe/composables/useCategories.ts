import { useToast } from '~/components/ui/toast'

interface Category {
  id: number
  name: string
}

const { toast } = useToast()
export const useCategories = () => {
  const categories = ref<Category[]>([])
  const isLoading = ref(false)

  const getCategories = async () => {
    isLoading.value = true
    try {
      const { data: categoriesData } = await useAPI<Category[]>(
        '/category/get-all',
        {
          method: 'GET'
        }
      )
      if (!categoriesData?.value || categoriesData.value.length === 0) {
        toast({
          title: 'No categories found',
          description: 'There are no categories available'
        })
        categories.value = []
      } else {
        categories.value = categoriesData.value
      }
    } catch (error) {
      toast({
        title: 'Error fetching categories',
        description: 'An error occurred while fetching categories.'
      })
      console.error('Error fetching categories:', error)
    } finally {
      isLoading.value = false
    }
  }

  const addCategory = async (category: Category) => {
    try {
      await useAPI('/category/add', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(category)
      })
      toast({
        title: 'Category added',
        description: `Category ${category.name} has been added`
      })
    } catch (error) {
      toast({
        title: 'Error adding category',
        description: 'An error occurred while adding the category.'
      })
      console.error('Error adding category:', error)
    }
  }
  const deleteCategory = async (id: number) => {
    try {
      await useAPI(`/category/delete/`, {
        method: 'DELETE',
        body: JSON.stringify({ id }),
        headers: { 'Content-Type': 'application/json' }
      })
      toast({
        title: 'Category deleted',
        description: `Category with ID ${id} has been deleted`
      })
    } catch (error) {
      toast({
        title: 'Error deleting category',
        description: 'An error occurred while deleting the category.'
      })
      console.error('Error deleting category:', error)
    }
  }

  const updateCategory = async (id: number, name: string) => {
    try {
      // Constructing the payload as { id, name }
      const payload = { id, name }

      await useAPI(`/category/update/`, {
        method: 'PUT',
        body: JSON.stringify(payload),
        headers: { 'Content-Type': 'application/json' }
      })

      toast({
        title: 'Category updated',
        description: `Category has been updated to ${name}`
      })
    } catch (error) {
      toast({
        title: 'Error updating category',
        description: 'An error occurred while updating the category.'
      })
      console.error('Error updating category:', error)
    }
  }

  return {
    categories,
    isLoading,
    getCategories,
    addCategory,
    deleteCategory,
    updateCategory
  }
}
