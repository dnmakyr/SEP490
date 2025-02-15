<script lang="ts" setup>
import { ref, defineEmits, watch } from 'vue'

const props = defineProps<{ open: boolean }>()
const emit = defineEmits(['close', 'create'])

const name = ref('')
const percent = ref<number | undefined>(undefined)

watch(
  () => props.open,
  (newVal) => {
    if (newVal) {
      name.value = ''
      percent.value = undefined
    }
  }
)

const createDiscount = () => {
  if (name.value && percent.value != null) {
    emit('create', { discountName: name.value, discountPercent: percent.value })
  }
}
</script>

<template>
  <Dialog
    :open="props.open"
    @click-outside="emit('close')"
    @close="emit('close')"
  >
    <DialogContent>
      <DialogHeader>
        <DialogTitle>Create Discount</DialogTitle>
        <Button
          variant="ghost"
          class="absolute top-2 right-2"
          @click="emit('close')"
        />
      </DialogHeader>

      <div class="grid gap-4 py-4">
        <div class="grid grid-cols-4 items-center gap-4">
          <Label for="name" class="text-right">Name</Label>
          <Input id="name" v-model="name" class="col-span-3" />
        </div>
        <div class="grid grid-cols-4 items-center gap-4">
          <Label for="percent" class="text-right">Percent</Label>
          <Input
            id="percent"
            v-model="percent"
            type="number"
            class="col-span-3"
          />
        </div>
      </div>

      <DialogFooter>
        <Button @click="emit('close')">Cancel</Button>
        <Button :disabled="!name || percent === null" @click="createDiscount">
          Create Discount
        </Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
