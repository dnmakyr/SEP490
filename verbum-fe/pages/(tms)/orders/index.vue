<script setup lang="ts">
import { columns } from '~/components/Orders/column';

useSeoMeta({
  title: 'Orders'
})
definePageMeta({
  layout: 'default'
})

const { orders, isLoading, getOrders } = useOrders()

onMounted(async () => {
  if (!orders.value.length) {
    await getOrders()
  }
})


</script>

<template>
  <LoadingSpinner v-if="isLoading"/>
  <LazyOrdersTable v-else :columns="columns" :data="orders" />
</template>
