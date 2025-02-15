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

    class: 'bg-gray-200',
    currentPage: 1,
    itemsPerPage: 5
  },
  {
    status: 'IN_PROGRESS',
    class: 'bg-emerald-200',
    currentPage: 1,
    itemsPerPage: 5
  },
  {
    status: 'SUBMITTED',
    class: 'bg-cyan-200',
    currentPage: 1,
    itemsPerPage: 5
  },
  {
    status: 'APPROVED',
    class: 'bg-cyan-400',
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
  return props.data.filter((job) => {
    const matchesStatus =
      job.status === status.status ||
      (status.status === 'NEW' && job.status === null)
    const matchesQuery =
      job.name &&
      job.name.toLowerCase().includes(searchQuery.value.toLowerCase())
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
  <div class="flex-1 flex-col mt-5">
    <Input
      id="default-search"
      v-model="searchQuery"
      type="search"
      placeholder="Enter job name ..."
      class="border border-primary w-1/3"
    />
    <div class="flex h-full gap-3 mt-2">
      <div
        v-for="item in allStatus"
        :key="item.status"
        class="flex flex-col w-1/4"
      >
        <div :class="item.class" class="p-2 rounded-xl h-5/6 overflow-y-auto">
          <p class="font-bold text-cyan-950 mb-3">
            {{ item.status }}
          </p>
          <div v-for="job in paginatedJobs(item)" :key="job.id">
            <JobsCard
              v-if="
                job.status === item.status ||
                (item.status === 'NEW' && job.status === null)
              "
              :data="job"
              @click="navigateTo(`/jobs/details/${job.id}`)"
            />
          </div>
        </div>
        <div class="flex justify-center mt-3">
          <div class="flex space-x-3">
            <Button
              variant="outline"
              :disabled="item.currentPage === 1 || totalPages(item) === 0"
              @click="previousPage(item)"
            >
              Previous
            </Button>
            <span class="text-center py-2 px-1"
              >Page {{ item.currentPage }} of {{ totalPages(item) }}</span
            >
            <Button
              variant="outline"
              :disabled="
                item.currentPage === totalPages(item) || totalPages(item) === 0
              "
              @click="nextPage(item)"
            >
              Next
            </Button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
