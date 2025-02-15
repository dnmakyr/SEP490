<script setup>
const {user} = useAuthStore()

</script>

<template>
  <LandingContainer>
    <header class="flex flex-col lg:flex-row justify-between items-center my-5">
      <NuxtImg src="../public/logo.png" class="object-cover h-16 w-80"/>
      
      <div>
        <div v-if="!user" class="hidden lg:flex items-center gap-4">
          <a href="/login">Log in</a>
          <LandingLink href="/signup" size="md">Sign up</LandingLink>
        </div>
      </div>
    </header>
  </LandingContainer>
</template>
