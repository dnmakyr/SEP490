<script lang="ts" setup>
import { ref } from 'vue'
import type { Discount } from '~/types/discount'
import { columns } from '~/components/Discounts/columns'
import { Button } from '@/components/ui/button'
import { useDiscounts } from '~/composables/useDiscount'

const { discounts, getDiscounts, createDiscount } = useDiscounts()

useSeoMeta({
  title: 'Discounts'
})

definePageMeta({
  layout: 'default'
})

onMounted(() => {
  if (!discounts.value.length) {
    getDiscounts()
  }
})

// Track dialog state
const isDialogOpen = ref(false)

// Handle creating a new discount
const handleCreateDiscount = async (newDiscount: Discount) => {
  const newDiscountItem = {
    discountId: newDiscount.discountId,
    discountName: newDiscount.discountName,
    discountPercent: newDiscount.discountPercent,
    isUpdate: true
  }
  if (!newDiscount) return
  await createDiscount(newDiscountItem)
  await getDiscounts()

  closeDialog()
}

// Handle dialog close
const closeDialog = () => {
  isDialogOpen.value = false
}

// watch(discounts, newDiscounts)
</script>

<template>
  <div>
    <Button class="mr-16 my-3 float-right" @click="isDialogOpen = true"
      >Create Discount
    </Button>

    <DiscountsTable :columns="columns" :data="discounts" />

    <!-- Create Discount Dialog -->
    <DiscountsCreateDialog
      :open="isDialogOpen"
      @close="closeDialog"
      @create="handleCreateDiscount"
    />
  </div>
</template>
