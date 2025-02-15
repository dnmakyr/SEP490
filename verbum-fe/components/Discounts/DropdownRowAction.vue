<script lang="ts" setup>
import { ref, defineEmits } from 'vue'
import Button from '@/components/ui/button/Button.vue'
import DropdownMenu from '@/components/ui/dropdown-menu/DropdownMenu.vue'
import DropdownMenuItem from '@/components/ui/dropdown-menu/DropdownMenuItem.vue'
import DropdownMenuTrigger from '@/components/ui/dropdown-menu/DropdownMenuTrigger.vue'
import UpdateDialog from './UpdateDialog.vue'
import ConfirmDialog from './ConfirmDialog.vue'
import type { Discount } from '~/types/discount'
// const token = useCookie('access_token')

const props = defineProps<{
  rowData: Discount // Receive row data as a prop
}>()

const showUpdateDialog = ref(false)
const showDeleteDialog = ref(false)
const emit = defineEmits(['delete', 'update']) // Emit events for actions

const openUpdateDialog = () => {
  showUpdateDialog.value = true
}
const openDeleteDialog = () => {
  showDeleteDialog.value = true
}

const closeUpdateDialog = () => {
  showUpdateDialog.value = false
}
const closeDeleteDialog = () => {
  showDeleteDialog.value = false
}

const handleUpdate = (updatedDiscount: Discount) => {
  emit('update', updatedDiscount) // Emit the updated discount
}
const handleDelete = (updatedDiscount: Discount) => {
  emit('delete', updatedDiscount) // Emit the updated discount
}
</script>
<template>
  <div>
    <DropdownMenu>
      <DropdownMenuTrigger as-child>
        <Button
          variant="ghost"
          class="flex h-8 w-8 p-0 data-[state=open]:bg-muted"
        >
          ...
          <span class="sr-only">Open menu</span>
        </Button>
      </DropdownMenuTrigger>

      <DropdownMenuContent align="end" class="w-[160px]">
        <DropdownMenuItem @click="openUpdateDialog"> Edit </DropdownMenuItem>
        <DropdownMenuItem @click="openDeleteDialog"> Delete </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>

    <UpdateDialog
      v-if="showUpdateDialog"
      :open="showUpdateDialog"
      :row-data="props.rowData"
      @close="closeUpdateDialog"
      @update="handleUpdate"
    />

    <ConfirmDialog
      v-if="showDeleteDialog"
      :open="showDeleteDialog"
      :row-data="props.rowData"
      @close="closeDeleteDialog"
      @update="handleDelete"
    />
  </div>
</template>
