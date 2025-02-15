<script setup lang="ts">
import { useCategories } from '~/composables/useCategories'

useSeoMeta({
  title: 'Categories'
})

const {
  categories,
  getCategories,
  addCategory,
  updateCategory,
  deleteCategory
} = useCategories()

const newCategoryName = ref('')
const selectedCategory = ref(null)
const isEditing = ref(false)

onMounted(() => {
  if (!categories.value.length) {
    getCategories()
  }
})

const addNewCategory = async () => {
  if (!newCategoryName.value.trim()) return
  await addCategory({ name: newCategoryName.value })
  newCategoryName.value = ''
  await getCategories()
}

const editCategory = (category) => {
  selectedCategory.value = category
  newCategoryName.value = category.name
  isEditing.value = true
}
const cancleEdit = () => {
  selectedCategory.value = null
  newCategoryName.value = ''
  isEditing.value = false
}

const updateExistingCategory = async () => {
  if (!selectedCategory.value || !newCategoryName.value.trim()) return
  await updateCategory(selectedCategory.value.id, newCategoryName.value)
  selectedCategory.value = null
  newCategoryName.value = ''
  isEditing.value = false
  await getCategories()
}
const removeCategory = async (id) => {
  await deleteCategory(id)
  await getCategories()
}
</script>

<template>
  <div>
    <div class="container mx-auto space-y-4">
      <!-- Input form -->
      <div class="flex space-x-2 justify-center">
        <Input v-model="newCategoryName" class="border p-2 rounded" type="text" placeholder="Enter category name" @keydown.enter="addNewCategory()" />
        <Button class="text-white p-2 rounded" @click="isEditing ? updateExistingCategory() : addNewCategory()">
          {{ isEditing ? 'Update Category' : 'Add Category' }}
        </Button>
        <Button :disabled="!isEditing ? true : false" @click="cancleEdit">
          Cancel
        </Button>
      </div>

      <!-- Categories list -->
      <div class="space-y-2">
        <div v-for="category in categories" :key="category.id"
          class="flex justify-between items-center border p-2 rounded">
          <span class="font-semibold">{{ category.name }}</span>
          <div class="space-x-2">
            <Button v-if="!isEditing" class="px-3 py-1 rounded" variant="outline" @click="editCategory(category)">
              Edit
            </Button>
            <Dialog>
              <DialogTrigger>
                <Button class="px-3 py-1 rounded" variant="outline">
                  Delete
                </Button>
              </DialogTrigger>
              <DialogContent>
                <DialogHeader>
                  <DialogTitle>Are you sure absolutely sure?</DialogTitle>
                  <DialogDescription>
                    This action cannot be undone.
                  </DialogDescription>
                </DialogHeader>
                <DialogFooter>
                  <DialogClose>
                    <Button @click="removeCategory(category.id)">
                      Yes
                    </Button>
                  </DialogClose>
                  <DialogClose>
                    <Button>
                      No
                    </Button>
                  </DialogClose>
                </DialogFooter>
              </DialogContent>
            </Dialog>

          </div>
        </div>
      </div>
    </div>
  </div>
</template>
