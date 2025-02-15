<script setup lang="ts">
import { Trash2, Heart } from 'lucide-vue-next'
import { useTaskStore } from '../../stores/TaskStore'

interface Task {
  id: number
  title: string
  isFav: boolean
}

const props = defineProps<{ task: Task }>()

const taskStore = useTaskStore()
</script>

<template>
  <div class="h-14 w-72 border shadow-md p-3 m-2 flex">
    <h3>{{ props.task.title }}</h3>
    <div class="flex ml-auto gap-2">
      <Trash2
        class="h-6 w-6 cursor-pointer"
        @click="taskStore.deleteTask(props.task.id)"
      />
      <Heart
        :class="[
          'h-6 w-6 cursor-pointer',
          props.task.isFav ? 'text-red-500' : ''
        ]"
        @click="taskStore.toggleFav(props.task.id)"
      />
    </div>
  </div>
</template>
