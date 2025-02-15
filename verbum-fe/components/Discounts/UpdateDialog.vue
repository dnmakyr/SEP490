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
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import type { Discount } from '~/types/discount'
import { useDiscounts } from '~/composables/useDiscount'

const props = defineProps<{
  open: boolean
  rowData: Discount // Receive row data as a prop
}>()

const emit = defineEmits(['close', 'update']) // Emit update event
const isOpen = ref(props.open)
const name = ref(props.rowData.discountName)
const percent = ref(props.rowData.discountPercent)
const { updateDiscount, getDiscounts } = useDiscounts()

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
  }
)

const updateDiscountItem = async () => {
  const updateDiscountItem = {
    discountId: props.rowData.discountId,
    discountName: name.value,
    discountPercent: percent.value,
    isUpdate: true
  }
  await updateDiscount(updateDiscountItem)
  await getDiscounts()
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
        <DialogTitle>Edit Discount</DialogTitle>
        <Button
          variant="ghost"
          class="absolute top-2 right-2"
          @click="closeDialog"
        />
      </DialogHeader>

      <div class="grid gap-4 py-4">
        <div class="grid grid-cols-4 items-center gap-4">
          <Label for="name" class="text-right">Name</Label>
          <Input id="name" v-model="name" class="col-span-3" />
        </div>
        <div class="grid grid-cols-4 items-center gap-4">
          <Label for="percent" class="text-right">Percent</Label>
          <Input id="percent" v-model="percent" class="col-span-3" />
        </div>
      </div>

      <DialogFooter>
        <Button @click="closeDialog">Cancel</Button>
        <Button @click="updateDiscountItem">Save changes</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
