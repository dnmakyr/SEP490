<template>
  <Card :class="className">
    <CardHeader>
      <CardDescription>Uploaded files</CardDescription>
    </CardHeader>
    <CardContent class="grid gap-3">
      <div
        v-for="(file, index) in props.files"
        :key="file.name"
        class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
      >
        <span class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500" />
        <div class="flex flex-col gap-1">
          <p class="text-sm font-medium leading-none">
            {{ file.name }}
          </p>
          <div class="flex gap-5 max-w-sm">
            <Progress :value="uploadProgress[index]" />
            <p class="text-sm font-medium leading-none">
              {{ props.uploadProgress[index] || 0 }}%
            </p>
          </div>
        </div>
      </div>
    </CardContent>
    <Button @click="onSave">Save</Button>
  </Card>
</template>

<script lang="ts" setup>
import { defineProps, type PropType } from 'vue'
import Progress from './ui/progress/Progress.vue';

const props = defineProps({
  files: {
    type: Array as PropType<File[]>,
    required: true
  },
  uploadProgress: {
    type: Array as PropType<number[]>,
    required: true
  },
  className: {
    type: String,
    default: ''
  },
  onSave: {
    type: Function as PropType<() => void>,
    required: true
  }
})
</script>
