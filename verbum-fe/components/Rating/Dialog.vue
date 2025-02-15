<script lang="ts" setup>
import { ref, watch, defineEmits } from 'vue'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog'
import type { Rating } from '~/composables/useRating';

const props = defineProps<{ 
  open: boolean,
  orderId: string
 }>()
const emit = defineEmits(['close', 'submit'])

const {createRating} = useRating();
const isOpen = ref(props.open)
const currentSection = ref(1) // Track current section (1, 2, or 3)
const rating : Rating = {
  orderId : props.orderId,
  inTime : 0,
  expectation: 0,
  issueResolved: 0,
  moreThought: ''
}
const ratingQuestion: { question: string; category: string; key: keyof Rating }[] = [
  {
    question: 'Was the service done in time?',
    category: 'Service Quality',
    key: 'inTime'
  },
  {
    question: `Was the translation's quality up to expectations?`,
    category: 'Service Quality',
    key: 'expectation'
  },
  {
    question: `Were all of your issues with the translation resolved?`,
    category: 'Service Quality',
    key: 'issueResolved'
  },
  // {
  //   question: `Were all of your questions answered?`,
  //   category: 'Customer Support'
  // },
  // {
  //   question: `Did you enjoy interacting with our staff?`,
  //   category: 'Customer Support'
  // },
  // {
  //   question: `Did you receive fast responses?`,
  //   category: 'Customer Support'
  // }
]

const feedback = ref('')

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
    currentSection.value = 1 // Reset to first section when dialog opens
  }
)

const closeDialog = () => {
  emit('close')
}

const nextSection = () => {
  if (currentSection.value < 2) currentSection.value += 1
}

const prevSection = () => {
  if (currentSection.value > 1) currentSection.value -= 1
}

const updateRating = (key: keyof Rating, value: number) => {
  (rating[key] as number) = value;
};

const submitFeedback = async() => {
  rating.moreThought = feedback.value;
await(createRating(rating))
  emit('submit', feedback.value)
  closeDialog()
}
</script>

<template>
  <Dialog :open="isOpen" @click-outside="closeDialog" @close="closeDialog">
    <DialogContent class=" h-[55vh] flex flex-col">
      <DialogHeader>
        <DialogTitle>Rating Our Services</DialogTitle>
        <DialogDescription>
          In this dialog, you have the opportunity to share feedback on your recent order. Rate your experience to help us improve our service and ensure you receive the best possible quality in the future.
        </DialogDescription>
        <hr>
      </DialogHeader>
      
      <div class="flex-1 overflow-y-auto">
        <div v-if="currentSection === 1">
          <p class="font-semibold text-xl text-cyan-600">Service Quality</p>
          <div v-for="ques in ratingQuestion" :key="ques.question">
            <RatingQuestion 
            v-if="ques.category === 'Service Quality'" 
            :question="ques.question" 
            :rating="rating[ques.key] as number"
              @update-rating="updateRating(ques.key, $event)"
            />
          </div>
        </div>

        <!-- <div v-if="currentSection === 2">
          <p class="font-semibold text-xl text-cyan-600">Customer Support</p>
          <div v-for="ques in ratingQuestion" :key="ques.question">
            <RatingQuestion v-if="ques.category === 'Customer Support'" :question="ques.question" />
          </div>
        </div> -->

        <div v-if="currentSection === 2">
            <p class="mb-2">Do you have any additional feedback or suggestions for us to improve our service?</p>
          <textarea
            v-model="feedback"
            placeholder="More of your thoughts (optional)"
            class="w-full p-3 border rounded-md text-gray-900 dark:text-gray-100 bg-white dark:bg-gray-700 border-gray-300 dark:border-gray-600 focus:border-blue-500 dark:focus:border-blue-500 focus:ring focus:ring-blue-200 dark:focus:ring-blue-800 focus:ring-opacity-50"
            rows="3"
          />
        </div>
      </div>
      <hr>
      <DialogFooter class="flex justify-end space-x-2 mt-4">
          <Button v-if="currentSection > 1" @click="prevSection">Previous</Button>
          <Button v-if="currentSection < 2" @click="nextSection">Next</Button>
          <Button v-if="currentSection === 2" @click="submitFeedback">Submit</Button>
          <Button variant="ghost" @click="closeDialog">Cancel</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
