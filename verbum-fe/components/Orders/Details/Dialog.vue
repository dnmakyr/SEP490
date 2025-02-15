<script lang="ts" setup>
import { z } from 'zod'


const props = defineProps<{
  orderId?: string | undefined
}>()

const { sendRejectOrder } = useOrders()

const rejectReasonSchema = z.object({
  reason: z
    .string()
    .min(1, 'Reject reason is required')
    .max(500, 'Reject reason cannot exceed 500 characters')
    .trim()
})

const reason = ref('')
const error = ref('')

const onSubmit = () => {
  try {
    const validated = rejectReasonSchema.parse({ reason: reason.value })
    if (props.orderId) {
      sendRejectOrder(props.orderId, validated.reason)
    }
    error.value = ''
  } catch (err) {
    if (err instanceof z.ZodError) {
      error.value = err.errors[0].message
    }
  }
}
</script>

<template>
  <Dialog>
    <DialogTrigger as-child>
      <Button variant="outline">Reject Order</Button>
    </DialogTrigger>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Reject Order</DialogTitle>
        <DialogDescription>
          Enter the response why we reject this order
        </DialogDescription>
      </DialogHeader>
      <form 
        class="flex flex-col gap-4"
        @submit.prevent="onSubmit">
        <div class="flex flex-col gap-2">
          <Label>Reason</Label>
          <Input 
            v-model="reason"
            :class="{ 'border-destructive': error }" 
          />
          <span v-if="error" class="text-sm text-destructive">
            {{ error }}
          </span>
        </div>
        <DialogFooter>
          <DialogClose>
            <Button variant="outline">Cancel</Button>
          </DialogClose>
          <Button type="submit" :disabled="!reason">Reject</Button>
        </DialogFooter>
      </form>
    </DialogContent>
  </Dialog>
</template>