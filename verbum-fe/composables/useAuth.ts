/* eslint-disable @typescript-eslint/no-explicit-any */
import { useToast } from '~/components/ui/toast'
import { ref} from 'vue'
import { decodeToken } from '~/lib/auth/auth'
import type { User } from '~/types/user'
const { toast } = useToast()

export const useAuth = () => {
  const user = ref<User | null>(null)
  const accessToken = ref<string | null | undefined>(null)
  const refreshToken = ref<string | null>(null)
  const authStore = useAuthStore()

  const config = useRuntimeConfig()
  const login = async (credentials: any) => {
    try {
      const response = await fetch(`${config.public.baseUrl}/api/auth/login`, {
        method: 'POST',
        body: JSON.stringify(credentials),
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
      })

      if (!response.ok && response.status === 400) {
        toast({
          title: 'Login error',
          description: 'Invalid email or password'
        })
      } else {
        toast({
          title: 'Login error',
          description: 'An error occurred while logging in'
        })
      }

      accessToken.value = useCookie('access_token').value

      if (!accessToken.value) {
        throw new Error('No access token found')
      }

      const decodedUser = decodeToken(accessToken.value as string)
      if (!decodedUser) {
        throw new Error('Invalid access token')
      }
      authStore.setUser(decodedUser)

      toast({
        title: 'Logged in',
        description: 'You have been logged in successfully'
      })
        navigateTo('/orders')
    } catch (error) {
      console.error('Login error:', error)
    }
  }

  const signup = async (credentials: any) => {
    try {
      const response = await fetch(`${config.public.baseUrl}/api/auth/signup`, {
        method: 'POST',
        body: JSON.stringify(credentials),
        headers: { 'Content-Type': 'application/json' }
      })

      if (!response.ok) {
        toast({
          title: 'Failed to sign up',
          description: 'An error occurred while signing up'
        })
        throw new Error('Failed to sign up')
      }
      if (response.status === 204) {
        toast({
          title: 'Account created',
          description:
            'Account created successfully.'
        })
        navigateTo(`/resend/${credentials.email}`)
      }
    } catch (error) {
      console.error('Signup error:', error)
    }
  }

  const logout = () => {
    accessToken.value = null
    refreshToken.value = null
    authStore.clearUser()
    navigateTo('/login')
  }

  const googleAuth = async () => {
    window.open(`${config.public.baseUrl}/auth/google-login`)
  }
  // Refresh token silently
  return {
    user: user.value,
    login,
    logout,
    signup,
    googleAuth
  }
}
