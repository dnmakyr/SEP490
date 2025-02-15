<script lang="ts" setup>
import type { Linguist } from '@/types/user'
import { Check } from 'lucide-vue-next'
import type { assigneeNames } from '~/types/job';
interface LinguistSelectorProps {
  linguists: Linguist[]
  assignedLinguists?: assigneeNames[]
  modelValue?: string[]
  error?: boolean
}

const props = defineProps<LinguistSelectorProps>()
const emit = defineEmits(['update:selected-linguists', 'update:modelValue'])
const isOpen = ref(false)
const searchQuery = ref('')
const dropdownRef = ref<HTMLElement | null>(null)

onMounted(() => {
  document.addEventListener('mousedown', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('mousedown', handleClickOutside)
})

const handleClickOutside = (event: MouseEvent) => {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

function toggleDropdown() {
  isOpen.value = !isOpen.value
}
const selectedLinguistsIdsArray = ref<string[]>(props.modelValue || [])

onMounted(() => {
  if (props.assignedLinguists) {
    selectedLinguistsIdsArray.value = [...props.assignedLinguists.map(linguist => linguist.id)]
  }
})

const handleLinguistSelect = (linguistId: string) => {
  const index = selectedLinguistsIdsArray.value.indexOf(linguistId)
  if (index > -1) {
    selectedLinguistsIdsArray.value.splice(index, 1)
  } else {
    selectedLinguistsIdsArray.value.push(linguistId)
  }
  emit('update:selected-linguists', [...selectedLinguistsIdsArray.value])
  emit('update:modelValue', [...selectedLinguistsIdsArray.value])
}
const isSelected = (linguistId: string) => selectedLinguistsIdsArray.value.includes(linguistId)

const filteredLinguists = computed(() =>
  props.linguists.filter(linguist =>
    linguist.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
)

const selectedLinguistsDisplay = computed(() => {
  if (selectedLinguistsIdsArray.value.length === 0) {
    return 'Select linguist'
  }
  const selectedLinguists = props.linguists
    .filter(linguist => selectedLinguistsIdsArray.value.includes(linguist.id))
    .map(linguist => linguist.name)
  return selectedLinguists.join(', ')
})

</script>
<template>
  <div ref="dropdownRef" class="relative">
    <div
      class="flex items-center justify-between border rounded-md px-1 py-1 cursor-pointer"
      :class="[
        props.error ? 'border-red-500' : 'border-gray-300',
      ]"
      aria-haspopup="listbox"
      :aria-expanded="isOpen ? 'true' : 'false'"
      aria-controls="language-list"
      @click="toggleDropdown"
    >
      <span>
        {{ selectedLinguistsDisplay }}
      </span>
    </div>
    <div
      v-if="isOpen"
      id="language-list"
      class="absolute mt-1 w-[16rem] border border-gray-300 bg-white shadow-lg rounded-md z-10 overflow-y-auto"
    >
      <div class="p-2">
        <Input
          v-model="searchQuery"
          placeholder="Search linguist"
          type="text"
        />
      </div>
      <div role="listbox" title="language-list" class="max-h-20 overflow-auto">
        <div
          v-for="linguist in filteredLinguists"
          :key="linguist.id"
          class="px-4 py-2 cursor-pointer hover:bg-gray-100 flex items-center gap-2"
          role="option"
          :aria-selected="isSelected(linguist.id) ? 'true' : 'false'"
          @click="() => {
            handleLinguistSelect(linguist.id)
          }"
        >
          <Check v-if="isSelected(linguist.id)" />
          {{ linguist.name }}
        </div>
      </div>
    </div>
  </div>
</template>
