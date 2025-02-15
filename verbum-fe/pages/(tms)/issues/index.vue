<script setup lang="ts">
import { useIssues } from '~/composables/useIssues'
import type { Issue } from '~/types/issues'

useSeoMeta({
  title: 'Issues'
})

const { isLoading, issues, getIssues, updateIssue } = useIssues()
const { user } = useAuthStore()
const currentUserRole = user?.role

onMounted(() => {
  if (!issues.value.length) {
    getIssues()
  }
})

const handleUpdate = async (updateIssues: Issue) => {
  await updateIssue(updateIssues)
}
</script>

<template>
  <div class="flex flex-col space-y-4">
    <IssuesCarousel :issues="issues" :role="currentUserRole"/>
    <h1 class="text-2xl font-semibold">All issues</h1>
    <IssuesTable
      :issues="issues"
      :role="currentUserRole"
      @update="handleUpdate"
    />
  </div>
</template>
