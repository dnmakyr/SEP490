<script lang="ts" setup>
import { columns } from '~/components/Works/columns'
import { useWorks } from '~/composables/useWorks'
useSeoMeta({
  title: 'Works'
})

definePageMeta({
  layout: 'default'
})

const { works, isLoading, getWorks } = useWorks()

// Fetch works on mount
onMounted(() => {
  getWorks()
})

</script>

<template>
  <LoadingSpinner v-if="isLoading" />
  <WorksTable v-else :columns="columns" :data="works" />
</template>
