<script>
export default {
  props: {
    className: {
      type: String,
      default: ''
    }
  },
  computed: {
    svgClass() {
      return `${this.className} animate-pulse rounded bg-gray-300`
    }
  }
}
</script>

<template>
  <svg :class="svgClass" />
</template>