<script lang="ts" setup>

import { useReceipt } from '~/composables/useReceipt'

const {receipts, getReceipts} = useReceipt();
useSeoMeta({
  title: 'Receipts'
})

definePageMeta({
  layout: 'default'
})

onMounted(async () => {
  if(!receipts.value.length){
    getReceipts();
  }
})

</script>

<template>
  <LoadingSpinner v-if="!receipts"/>
     <ReceiptsTable v-else :receipts="receipts"/>
</template>
