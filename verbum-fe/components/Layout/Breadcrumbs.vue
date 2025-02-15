<script setup>
const route = useRoute()

const breadcrumb = computed(() => {
  const firstSegment = route.path.split('/').filter((segment) => segment)[0]
  return firstSegment ? firstSegment.replace(/-/g, ' ') : ''
})
</script>

<template>
  <h1 class="text-2xl text-primary font-bold capitalize">
    {{ breadcrumb }}
  </h1>
</template>
