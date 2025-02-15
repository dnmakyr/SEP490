<script lang="ts" setup>
import { ref, watch, defineEmits } from 'vue'
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from '@/components/ui/dialog'
import type { Discount } from '~/types/discount'
import { useDiscounts } from '~/composables/useDiscount'

const { deleteDiscount } = useDiscounts()
const props = defineProps<{
  open: boolean
  rowData: Discount // Receive row data as a prop
}>()

const emit = defineEmits(['close', 'delete']) // Emit update event
const isOpen = ref(props.open)

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
  }
)

const deleteDiscountItem = async () => {
  await deleteDiscount(props.rowData.discountId)
  closeDialog()
}

const closeDialog = () => {
  emit('close') // Emit close event
}
</script>

<template>
  <Dialog :open="isOpen" @click-outside="closeDialog" @close="closeDialog">
    <DialogContent class="sm:max-w-[425px]">
      <DialogHeader>
        <DialogTitle>Are you sure want to delete ?</DialogTitle>
        <Button
          variant="ghost"
          class="absolute top-2 right-2"
          @click="closeDialog"
        />
      </DialogHeader>
      <DialogFooter>
        <Button @click="closeDialog">Cancel</Button>
        <Button @click="deleteDiscountItem">Delete Discount</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
