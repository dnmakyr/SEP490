<template>
  <Dialog>
    <DialogTrigger as-child>
      <slot />
    </DialogTrigger>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Reject Reason</DialogTitle>
        <Input 
          v-model="reason" 
          placeholder="Enter reject reason" 
          :class="{ 'border-red-500': error }"
        />
        <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
      </DialogHeader>
    <DialogFooter>
      <DialogClose>
        <Button variant="outline" @click="reason = ''">Cancel</Button>
      </DialogClose>
      <Button :disabled="!reason" @click="reject">Reject</Button>
    </DialogFooter>
  </DialogContent>
  </Dialog>
</template>

<script lang="ts" setup>
import { z } from 'zod'

const rejectReasonSchema = z.object({
  reason: z
    .string()
    .min(1, 'Reject reason is required')
    .max(500, 'Reject reason cannot exceed 500 characters')
    .trim()
})

const reason = ref('')
const error = ref('')

const emit = defineEmits(['reject'])
const reject = () => {
  try {
    const validated = rejectReasonSchema.parse({ reason: reason.value })
    emit('reject', validated.reason)
    error.value = ''
  } catch (err) {
    if (err instanceof z.ZodError) {
      error.value = err.errors[0].message
    }
  }
}
</script>

<style></style>