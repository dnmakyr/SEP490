<script setup lang="ts">
import { ref } from 'vue'
import type { Job } from '~/types/job'

// Define the type for the status object
interface Status {
  status: string
  class: string
  currentPage: number
  itemsPerPage: number
}

const props = defineProps<{
  data: Job[]
}>()

const allStatus = ref<Status[]>([
  {
    status: 'NEW',
    class: 'bg-cyan-200 text-black',
    currentPage: 1,
    itemsPerPage: 5 // Set the number of items to show per page for each status
  },
  {
    status: 'IN_PROGRESS',
    class: 'bg-teal-200 text-black',
    currentPage: 1,
    itemsPerPage: 5
  },
  {
    status: 'SUBMITTED',
    class: 'bg-emerald-200 text-black',
    currentPage: 1,
    itemsPerPage: 5
  },
  {
    status: 'APPROVED',
    class: 'bg-gray-200 text-black',
    currentPage: 1,
    itemsPerPage: 5
  }
])

// Search query for filtering works by name
const searchQuery = ref<string>('')

// Function to calculate total pages for a specific status
const totalPages = (status: Status) => {
  const filteredJobs = filteredJobsByStatus(status)
  return Math.ceil(filteredJobs.length / status.itemsPerPage)
}

// Function to filter works based on status and search query
const filteredJobsByStatus = (status: Status) => {
  return props.data.filter(job => {
    const matchesStatus =
      job.status === status.status ||
      (status.status === 'NEW' && job.status === null)
    const matchesQuery = job.name && job.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    return matchesStatus && matchesQuery
  })
}

// Function to get paginated works for a specific status
const paginatedJobs = (status: Status) => {
  const filteredJobs = filteredJobsByStatus(status)
  const start = (status.currentPage - 1) * status.itemsPerPage
  return filteredJobs.slice(start, start + status.itemsPerPage)
}

// Navigation functions for changing pages
const nextPage = (status: Status) => {
  if (status.currentPage < totalPages(status)) {
    status.currentPage++
  }
}

const previousPage = (status: Status) => {
  if (status.currentPage > 1) {
    status.currentPage--
  }
}
</script>

<template>
  <div class="relative w-2/6 mb-3">
    <input
      id="default-search"
      v-model="searchQuery"
      type="search"
      class="block w-full p-4 ps-10 text-sm text-gray-900 border border-gray-300 rounded-xl bg-gray-50 focus:ring-blue-500 focus:border-blue-500 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
      placeholder="Enter job name ...">
    <button
      type="submit"
      class="text-white absolute end-2.5 bottom-2.5 bg-cyan-600 hover:bg-cyan-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-4 py-2 dark:bg-blue-600 dark:hover:bg-blue-700 dark:focus:ring-blue-800">
      Search
    </button>
  </div>
  <div class="flex flex-row gap-3 mt-2">
    <div v-for="item in allStatus" :key="item.status" class="flex flex-col w-1/4 statusCol">
      <p class="font-bold text-cyan-950 text-center mb-3">
        {{ item.status }}
      </p>
      <div :class="item.class" class="p-2 rounded-xl h-5/6 overflow-y-auto">
        <div v-for="job in paginatedJobs(item)" :key="job.id">
            <JobsCard
              v-if="job.status === item.status || (item.status === 'NEW' && job.status === null)"
              :data="job"
              @click="useRouter().push(`/jobs/details/${job.id}`)"
            />
        </div>
      </div>
      <div class="flex justify-center">
        <div class="flex space-x-3 mt-4">
          <Button
            variant="outline"
            :disabled="item.currentPage === 1 || totalPages(item) === 0"
            @click="previousPage(item)"
          >
            Previous
          </Button>
          <span class="text-center py-2 px-1">Page {{ item.currentPage }} of {{ totalPages(item) }}</span>
          <Button
            variant="outline"
            :disabled="item.currentPage === totalPages(item) || totalPages(item) === 0"
            @click="nextPage(item)"
          >
            Next
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>

<style>
.statusCol {
  height: 80vh;
}
</style>
