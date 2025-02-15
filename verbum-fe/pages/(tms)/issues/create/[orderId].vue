<script lang="ts" setup>
useSeoMeta({
  title: 'Create Issues'
})

const route = useRoute()
const orderId = route.params.orderId
</script>

<template>
  <IssuesCreateForm :orderId="orderId" />
</template>
