<template>
  <Dialog>
    <DialogTrigger>
      <slot />
    </DialogTrigger>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Are you absolutely sure?</DialogTitle>
        <DialogDescription>
          This action cannot be undone.
        </DialogDescription>
      </DialogHeader>
      <DialogFooter>
        <Button variant="outline">No</Button>
        <Button variant="default" @click='accept'>Yes</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>

</template>

<script lang="ts" setup>
const props = defineProps<{
  orderId: string
}>()

const { changeOrderStatus } = useOrders()

const accept = async () => {
  await changeOrderStatus(props.orderId, 'ACCEPTED')
}
</script>