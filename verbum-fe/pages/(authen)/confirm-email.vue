<template>
  <div class="confirm-email-page">
    <h1>Confirming your email...</h1>
    <p v-if="loading">Please wait while we confirm your email.</p>
    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="success">{{ successMessage }}</p>
  </div>
</template>

<script setup>
const config = useRuntimeConfig()

useSeoMeta({
  title: 'Confirm Email'
})
definePageMeta({
  layout: false
})

const route = useRoute()
const loading = ref(true)
const success = ref(false)
const successMessage = ref('')
const error = ref('')

// Function to confirm email
async function confirmEmail(token) {
  try {
    const response = await fetch(
      `${config.public.baseUrl}/api/auth/confirm-email/?access_token=${token}`,
      {
        method: 'get'
      }
    )

    if (response.ok) {
      success.value = true
      successMessage.value = 'Your email has been successfully confirmed!'
      const router = useRouter()
      setTimeout(() => {
        router.push('/login')
      }, 3000)
    } else {
      throw new Error(data.message || 'Email confirmation failed.')
    }
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
}

// On mounted, get the access_token from the query and confirm the email
onMounted(() => {
  const token = route.query.access_token

  if (token) {
    confirmEmail(token)
  } else {
    error.value = 'No access token found in the URL.'
    loading.value = false
  }
})
</script>

<style scoped>
.confirm-email-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
  height: 100vh;
}

.error {
  color: red;
}
</style>
