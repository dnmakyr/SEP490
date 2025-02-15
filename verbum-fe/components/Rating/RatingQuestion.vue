<script lang="ts" setup>
const props = defineProps<{ question: string; rating: number }>();
const emit = defineEmits(['update-rating']);
const rating = ref(props.rating);

const updateRating = (value: number) => {
  rating.value = value;
  emit('update-rating', value); // Emit the new rating value to the parent
};
</script>
<template>
  <div>
    <div>
      <p>{{ question }}</p>
    </div>
    <div class="flex justify-center space-x-2 mb-6">
      <button
        v-for="star in 5"
        :key="star"
        :class="[
          'text-3xl focus:outline-none',
          rating >= star ? 'text-yellow-400' : 'text-gray-300'
        ]"
        @click="updateRating(star)"
      >
        ★
      </button>
    </div>
  </div>
</template>
