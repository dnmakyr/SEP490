<script setup>
import DualListBox from '~/components/Languages/DualListBox.vue'
import { useLanguages } from '~/composables/useLanguages'

const {
  languages,
  supportedLanguages,
  getLanguages,
  getSupportedLanguages,
  updateSupportedLanguages
} = useLanguages()
useSeoMeta({
  title: 'Languages'
})

definePageMeta({
  layout: 'default'
})

onMounted(() => {
  if (!languages.value.length && !supportedLanguages.value.length) {
    getLanguages()
    getSupportedLanguages()
  }
})

const selects = []

const handleSave = async () => {
  const updatedItems = selects.map((item) => ({
    languageId: item.languageId,
    support: item.support
  }))

  await updateSupportedLanguages(updatedItems)
  await getLanguages()
  await getSupportedLanguages()

  selects.length = 0
}

const handleCancel = async () => {
  await getLanguages()
  await getSupportedLanguages()
  selects.length = 0
}
</script>

<template>
  <div>
    <DualListBox
      title-available-items="Unsupported Languages"
      title-selected-items="Supported Languages"
      :selects="selects"
      :available-items-list="languages"
      :selected-items-list="supportedLanguages"
      @save="handleSave"
      @cancel="handleCancel"
    />
    <Toaster />
  </div>
</template>
