<template>
    <AlertDialog>
      <AlertDialogTrigger><slot/></AlertDialogTrigger>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
          <AlertDialogDescription>
            This action cannot be undone. We will not refund the deposit in the circumstance that it have been done.
          </AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel>No</AlertDialogCancel>
          <AlertDialogAction @click="changeOrderStatus(orderId, 'CANCELLED')">Yes</AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
</template>

<script lang="ts" setup>
defineProps({
  orderId: {
    type: String,
    required: true
  }
})

const { changeOrderStatus } = useOrders()
</script>

<style>

</style>