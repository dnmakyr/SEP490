<script setup lang="ts">
import * as z from 'zod'
import { Button } from '@/components/ui/button'
import { AutoForm } from '@/components/ui/auto-form'

const config = useRuntimeConfig()

const PASSWORD_REGEX =
  /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@.#$!%*?&^])[A-Za-z\d@.#$!%*?&]{8,12}$/

const schema = z
  .object({
    name: z.string(),
    email: z.string().email(),
    password: z.string().min(8).max(12).regex(PASSWORD_REGEX, {
      message:
        'Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.'
    }),
    confirm: z.string()
  })
  .refine((data) => data.password === data.confirm, {
    message: 'Passwords must match.',
    path: ['confirm']
  })

const form = useForm({
  validationSchema: toTypedSchema(schema)
})

const onSubmit = async (values: Record<string, string>) => {
  try {
    const body = {
      name: values.name,
      email: values.email,
      password: values.password,
      roleCode: 'CLIENT'
    }
    await useAuth().signup(body)
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
        <h1 class="text-3xl font-bold">Sign up</h1>
        <p class="text-balance text-muted-foreground">
          Enter your email below to create your account
        </p>
      </div>
      <AutoForm
        class="space-y-3"
        :form="form"
        :schema="schema"
        :field-config="{
          name: {
            inputProps: {
              type: 'text',
              placeholder: 'Enter your name'
            }
          },
          email: {
            inputProps: {
              type: 'email',
              placeholder: 'email@example.com'
            }
          },
          password: {
            inputProps: {
              type: 'password',
              placeholder: 'Enter your password'
            }
          },
          confirm: {
            label: 'Confirm Password',
            inputProps: {
              type: 'password',
              placeholder: 'Re-enter your password'
            }
          }
        }"
        @submit="onSubmit"
      >
        <Button type="submit" class="w-full"> Sign up </Button>
      </AutoForm>
      <Button variant="outline" class="w-full" @click.prevent="googleAuth()">
        <NuxtImg
          src="https://i.pinimg.com/originals/ca/2f/7d/ca2f7db280e9c773e341589a81c15082.png"
          width="20"
        />
        <span class="px-3">Continue with Google</span>
      </Button>
      <div class="mt-4 text-center text-sm">
        Already have an account?
        <a href="/login" class="underline text-primary">Login</a>
      </div>
    </div>
    <Toaster/>
  </div>
</template>
