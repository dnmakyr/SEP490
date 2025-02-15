import auth from '~/middlewares/auth';
<script setup lang="ts">
useSeoMeta({
  title: 'Login'
})
definePageMeta({
  layout: 'auth'
})
</script>

<template>
  <ClientOnly>
    <AuthLoginForm />
  </ClientOnly>
</template>
