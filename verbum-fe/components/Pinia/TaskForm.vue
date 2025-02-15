<script setup>
const taskStore = useTaskStore()

const newTask = ref('')

const handleSubmit = () => {
  if (newTask.value.length > 0) {
    taskStore.addTask({
      title: newTask.value,
      isFav: false,
      id: Math.floor(Math.random() * 1000000).toString()
    })
    newTask.value = ''
  }
  newTask.value = ''
}
</script>

<template>
  <form class="flex flex-col items-center gap-2" @submit.prevent="handleSubmit">
    <Input v-model="newTask" type="text" placeholder="I need to..." />
    <Button class="w-16">Add</Button>
  </form>
</template>
