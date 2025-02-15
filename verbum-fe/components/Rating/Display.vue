<script lang="ts" setup>
defineProps<{ question: string; rating: number }>();
</script>
<template>
  <div>
    <div class="flex justify-center space-x-2">
      <div class="w-[8rem] mt-2">
        <p>{{ question }}</p>
      </div>
      <div class="flex-auto flex flex-row">
        <p
          v-for="star in 5"
          :key="star"
          :class="[
            'text-2xl focus:outline-none',
            rating >= star ? 'text-yellow-400' : 'text-gray-300'
          ]"
        >
          ★
        </p>

      </div>
    </div>
  </div>
</template>
