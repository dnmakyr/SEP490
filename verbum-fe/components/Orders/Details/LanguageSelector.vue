<script setup lang="ts">
interface Language {
  languageId: string
  languageName: string
  support: boolean
}

interface LanguageSelectorProps {
  languageList: Language[]
  selectedLanguages: string[] | string
  originalLanguages: string[] | string
  isSourceLanguage: boolean
}

const props = defineProps<LanguageSelectorProps>()
const emit = defineEmits(['update:selected-languages'])

const isOpen = ref(false)
const searchQuery = ref('')
const dropdownRef = ref<HTMLElement | null>(null)

const selectedLanguagesArray = computed(() =>
  Array.isArray(props.selectedLanguages)
    ? props.selectedLanguages
    : props.selectedLanguages
      ? [props.selectedLanguages]
      : []
)

onMounted(() => {
  document.addEventListener('mousedown', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('mousedown', handleClickOutside)
})

function handleClickOutside(event: MouseEvent) {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

function toggleDropdown() {
  isOpen.value = !isOpen.value
}

function handleLanguageSelect(languageId: string) {
  if (props.isSourceLanguage) {
    emit('update:selected-languages', languageId)
  } else {
    const currentSelection = [...selectedLanguagesArray.value]
    const index = currentSelection.indexOf(languageId)

    if (index > -1) {
      currentSelection.splice(index, 1)
    } else {
      currentSelection.push(languageId)
    }

    emit('update:selected-languages', currentSelection)
  }

  if (props.isSourceLanguage) {
    isOpen.value = false
  }
}

const isSelected = (languageId: string) =>
  props.isSourceLanguage
    ? props.selectedLanguages === languageId
    : selectedLanguagesArray.value.includes(languageId)

const filteredLanguages = computed(() =>
  props.languageList.filter(
    (lang) =>
      lang.languageName
        .toLowerCase()
        .includes(searchQuery.value.toLowerCase()) ||
      lang.languageId.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
)
</script>

<template>
  <div ref="dropdownRef" class="relative">
    <div
      class="flex items-center justify-between rounded-md px-1 py-1 cursor-pointer"
      aria-haspopup="listbox"
      :aria-expanded="isOpen ? 'true' : 'false'"
      aria-controls="language-list"
      @click="toggleDropdown"
    >
    <span>
      <template v-if="props.isSourceLanguage">
        <div class="p-2 border rounded-md">
          <Badge>
          {{ languageList.find((lang)=> lang.languageId === $props.selectedLanguages)?.languageName || 'Select Source Language' }}
          </Badge>
        </div>
      </template>
      <template v-else>
        <template v-if="selectedLanguagesArray.length">
          <div class="p-2 border rounded-md space-x-2">
            <Badge 
            v-for="(languageName, languageId) in selectedLanguagesArray" 
            :key="languageId"
            class="bg-slate-500"
          >
            {{ languageList.find((lang) => lang.languageId===languageName)?.languageName }}
            </Badge>
        </div>
        </template>
        <Badge v-else>Select Target Language</Badge>
      </template>
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
          type="text"
          placeholder="Search languages..."
          class="w-full border border-gray-300 rounded-md px-3 py-2"
          aria-label="Search languages"
        />
      </div>
      <div role="listbox" title="language-list" class="max-h-20 overflow-auto">
        <div
          v-for="lang in filteredLanguages"
          :key="lang.languageId"
          class="px-4 py-2 cursor-pointer"
          :class="{ 'bg-gray-200': isSelected(lang.languageId) }"
          role="option"
          :aria-selected="isSelected(lang.languageId) ? 'true' : 'false'"
          @click="handleLanguageSelect(lang.languageId)"
        >
          <span>{{ lang.languageName }}</span>
        </div>
      </div>
    </div>
  </div>
</template>
