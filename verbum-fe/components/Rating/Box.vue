<script lang="ts" setup>
import { CircleUser } from 'lucide-vue-next'
import { ref, watch, onMounted } from 'vue'
import { useRating } from '~/composables/useRating' // Ensure the composable is correctly imported

const { getRatingByOrderId, filteredRating, deleteRating, updateRating } = useRating()

const props = defineProps<{ orderId: string }>()


// States for editing and storing the current rating
const isEditing = ref(false)
const editedRating = ref<Rating>({
  orderId: props.orderId,
  inTime: 0,
  expectation: 0,
  issueResolved: 0,
  moreThought: '',
})

async function refreshPage() {
  window.location.reload()
}

// Watch for changes in filteredRating and update editedRating accordingly
watch(
  () => filteredRating.value,
  (newRating) => {
    if (newRating) {
      Object.assign(editedRating.value, newRating)
    }
  },
  { immediate: true }
)

// Fetch the rating data on mount
onMounted(() => {
  getRatingByOrderId(props.orderId)
})

// Enable editing mode
const handleEditRating = () => {
  isEditing.value = true
}

// Cancel editing mode
const cancelEdit = () => {
  isEditing.value = false
}

// Save the updated rating and refresh the display
const saveEdit = async () => {
  try {
    await updateRating({
      ...editedRating.value,
      ratingId: filteredRating.value?.ratingId,
    })

    // Refresh the filtered rating after saving
    await getRatingByOrderId(props.orderId)

    // Exit editing mode
    isEditing.value = false
    refreshPage()
  } catch (error) {
    console.error('Error saving edited rating:', error)
  }
}

// Confirm and delete the rating
const confirmAndDeleteRating = async () => {
  if (confirm('Are you sure you want to delete this rating? This action cannot be undone.')) {
    try {
      if (filteredRating.value?.ratingId) {
        await deleteRating(filteredRating.value.ratingId)
        // Clear the rating after deletion
        await getRatingByOrderId(props.orderId)
        refreshPage()
      }
    } catch (error) {
      console.error('Failed to delete rating:', error)
    }
  }
}
</script>

<template>
  <div v-if="filteredRating" class="border rounded-md p-5">
    <div class="flex gap-2">
      <div>
        <CircleUser class="h-8 w-8 mt-1" />
      </div>
      <div>
        <p class="text-[1.5rem] font-semibold">Your Review</p>
        <template v-if="!isEditing">
          <div>
            <RatingDisplay :question="'Expectation'" :rating="filteredRating.expectation" />
            <RatingDisplay :question="'In Time'" :rating="filteredRating.inTime" />
            <RatingDisplay :question="'Issues Resolved'" :rating="filteredRating.issueResolved" />
          </div>
          <div class="mt-3">
            <span class="font-bold">Comment:</span> {{ filteredRating.moreThought }}
          </div>
        </template>

        <!-- Edit Form -->
        <template v-else>
          <div>
            <div class="mb-3">
              <label class="font-bold">Expectation:</label>
              <Input v-model="editedRating.expectation" type="number" class="w-full" />
            </div>
            <div class="mb-3">
              <label class="font-bold">In Time:</label>
              <Input v-model="editedRating.inTime" type="number" class="w-full" />
            </div>
            <div class="mb-3">
              <label class="font-bold">Issues Resolved:</label>
              <Input v-model="editedRating.issueResolved" type="number" class="w-full" />
            </div>
            <div class="mb-3">
              <label class="font-bold">Comment:</label>
              <Textarea v-model="editedRating.moreThought" class="w-full" rows="3" />
            </div>
          </div>
        </template>
      </div>
    </div>

    <div class="flex flex-row gap-2 mt-5">
      <template v-if="!isEditing">
        <Button class="bg-gray-500" @click="confirmAndDeleteRating">Delete</Button>
        <Button class="bg-blue-500" @click="handleEditRating">Edit</Button>
      </template>
      <template v-else>
        <Button class="bg-green-500" @click="saveEdit">Save</Button>
        <Button class="bg-gray-500" @click="cancelEdit">Cancel</Button>
      </template>
    </div>
  </div>
</template>
