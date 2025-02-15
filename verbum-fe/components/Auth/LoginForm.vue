<!-- eslint-disable @typescript-eslint/no-unused-vars -->
<script setup lang="ts">
import * as z from 'zod'

const config = useRuntimeConfig()

const schema = z.object({
  email: z
    .string()
    .email('Invalid email address')
    .min(3, 'Email is too short')
    .nonempty('Email is required'),
  password: z.string().min(6, 'Password is too short')
})

const form = useForm({
  validationSchema: toTypedSchema(schema)
})
// const { toast } = useToast()
const onSubmit = async (values: { email: string; password: string }) => {
  try {
    // await useAuth().login(values)
    await useAuth().login({
      email: values.email,
      password: values.password
    })
  } catch (error) {
    console.error(error)
  }
}
const googleAuth = () => {
  window.open(`${config.public.baseUrl}/api/auth/google-login`)
}
</script>

<template>
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      <div class="grid gap-2 text-center">
        <h1 class="text-3xl font-bold">Login</h1>
        <p class="text-balance text-muted-foreground">
          Enter your email below to login to your account
        </p>
      </div>
      <AutoForm
        class="space-y-3"
        :form="form"
        :schema="schema"
        :field-config="{
          email: {
            inputProps: {
              type: 'email',
              placeholder: 'email@example.com',
              autocomplete: 'email'
            }
          },
          password: {
            inputProps: {
              type: 'password',
              placeholder: 'Enter your password',
              autocomplete: 'current-password'
            }
          }
        }"
        @submit="onSubmit"
      >
        <Button type="submit" class="w-full"> Login </Button>
      </AutoForm>
      <Button variant="outline" class="w-full" @click.prevent="googleAuth()">
        <NuxtImg
          src="https://i.pinimg.com/originals/ca/2f/7d/ca2f7db280e9c773e341589a81c15082.png"
          width="20"
        />
        <span class="px-3">Continue with Google</span>
      </Button>
      <div class="mt-4 text-center text-sm">
        Don't have an account?
        <a href="/signup" class="underline text-primary">Sign up</a>
      </div>
    </div>
    <Toaster />
  </div>
</template>
