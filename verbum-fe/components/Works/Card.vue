<script setup lang="ts">
import type { Work } from '~/composables/useWorks';

const props = defineProps<{
  data: Work
}>();
</script>

<template>
  <div class="bg-slate-50 m-1 mb-3 rounded-lg p-2 h-24">
    <div class="font-semibold">
      {{ props.data.workName }}
    </div>
    <!-- <div>
      this is description!!
    </div> -->
    <div>
      {{ props.data.sourceLanguageId }}
    </div>
  </div>
</template>