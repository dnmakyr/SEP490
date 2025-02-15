<template>
  <Dialog>
    <DialogTrigger as-child>
      <slot />
    </DialogTrigger>
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Are you sure?</DialogTitle>
        <DialogDescription>
          This action cannot be undone.
        </DialogDescription>
      </DialogHeader>
      <DialogFooter>
        <DialogClose>
          <Button variant="outline">Cancel</Button>
        </DialogClose>
        <Button variant="default" @click="approve">
          Approve
        </Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>

</template>

<script lang="ts" setup>
const emit = defineEmits(['approve'])

const approve = () => {
  emit('approve')
}
</script>

<style></style>