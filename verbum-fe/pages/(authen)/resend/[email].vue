<template>
    <div class="flex justify-center items-center flex-col h-screen space-y-3">
        <h1 class="font-bold text-4xl">Check your inbox</h1>
        <span class="flex flex-col justify-center items-center space-x-2">
            <p>Click on the confirmation button in the email we sent to</p>
            <p class="font-semibold">{{ email }}</p>
        </span>
        <Button variant="outline" class="rounded-full space-x-1" @click="navigateTo('/login')">
            <MoveLeft class="w-4 h-4 text-green-500" />
            <h1 class="font-semibold">Go To Login</h1>
        </Button>
        <span class="flex flex-col items-center">
            <h1>
                Didn't receive the email? Check your Spam folder or
            </h1>
            <h1>
                <a class="text-blue-500 underline hover:cursor-pointer" @click="resend">Resend the email</a>
            </h1>
        </span>
    </div>
</template>

<script setup>
import { MoveLeft } from 'lucide-vue-next'
const route = useRoute()
const email = ref(route.params.email)
const baseUrl = useRuntimeConfig().public.baseUrl
const resend = async () => {
    await fetch(`${baseUrl}/api/auth/resend-email?email=${email.value}`, {
        method: 'POST',
    })
}

useSeoMeta({
    title: 'Resend email',
})
definePageMeta({
    layout: false
})
</script>