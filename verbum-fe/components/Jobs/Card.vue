<script setup lang="ts">
import type { Job } from '~/types/job';

const props = defineProps<{
  data: Job
}>();
const emit = defineEmits(['click:card'])
useHead({
  title: "Works Details"
})
</script>

<template>
  <div
    class="bg-slate-50 p-1 mb-3 rounded-lg h-20 w-full max-w-md hover:bg-slate-100 cursor-pointer"
    @click="emit('click:card')"
  >
    <div class="font-semibold truncate" :title="props.data.name">
      {{ getJobName(props.data.name) }}
    </div>
    <div>
      Target Language: {{ props.data.targetLanguageId }}
    </div>
  </div>
</template>
