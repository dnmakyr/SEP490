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

const props = defineProps<{
  title: string
  description: string
  open: boolean
}>()

const emit = defineEmits(['close', 'confirm', 'refresh']) // Emit update event
const isOpen = ref(props.open)

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
  }
)

const closeDialog = () => {
  emit('close') // Emit close event
}

function handleConfirm() {
  emit('confirm');
  emit('refresh');
}
</script>

<template>
  <Dialog :open="isOpen" @click-outside="closeDialog" @close="closeDialog">
    <DialogContent class="sm:max-w-[425px]">
      <DialogHeader>
        <DialogTitle>{{ title }}</DialogTitle>
        <DialogDescription> {{ description }}</DialogDescription>
        <Button
          variant="ghost"
          class="absolute top-2 right-2"
          @click="closeDialog"
        />
      </DialogHeader>
      <DialogFooter>
        <Button class="bg-slate-500 hover:bg-slate-600" @click="$emit('close')"
          >Cancel</Button
        >
        <Button @click="handleConfirm">Confirm</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
