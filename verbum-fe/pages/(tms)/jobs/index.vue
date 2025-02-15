<script setup lang="ts">
import { getColumns } from '~/components/Jobs/column'
useSeoMeta({
  title: 'Jobs'
})

const { jobs, isLoading, getJobs } = useJobs()
const { assignList, getAssignList } = useUsers()
const role = useAuthStore().user?.role

const columns = getColumns(role)

onMounted(() => {
  getJobs()
  if (role?.includes('MANAGER')) {
    getAssignList()
  }
})
provide('assignList', assignList)
</script>

<template>
  <LoadingSpinner v-if="isLoading" />
  <JobsTable v-else :columns="columns" :data="jobs" />
</template>

<style scoped></style>
