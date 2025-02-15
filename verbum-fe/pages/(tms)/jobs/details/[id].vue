<script lang="ts" setup>
const route = useRoute()
const jobId = route.params.id as string

const { isLoading, job, getJobsDetail } = useJobs()
const { assignList, getAssignList } = useUsers()
const role = useAuthStore().user?.role as string | undefined

onMounted(() => {
  getJobsDetail(jobId)
  if (role?.includes('MANAGER')) {
    getAssignList()
  }
})
provide('assignList', assignList)

const jobTitle = computed(() => {
  if (!job.value) return 'Job Details'
  return getJobName(job.value.name)
})

useHead({
  title: jobTitle,
})
</script>
<template>
    <LoadingSpinner v-if="isLoading"/>
    <JobsDetails v-else :job="job" :role="role" />
</template>

<style>

</style>