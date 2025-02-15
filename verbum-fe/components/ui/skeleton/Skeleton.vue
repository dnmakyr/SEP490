<template>
  <div aria-live="polite" aria-busy="true" :class="className">
    <span class="inline-flex h-full w-full animate-pulse select-none rounded-md bg-gray-300 leading-none">
      ‌
    </span>
    <br />
  </div>
</template>

<script>
export default {
  props: {
    className: {
      type: String,
      default: ''
    }
  }
}
</script>