import { useToast } from '~/components/ui/toast'

const { toast } = useToast()

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  const access_token = useCookie('access_token')

  const api = $fetch.create({
    baseURL: config.public.baseUrl + '/api',
    async onRequest({ options }) {
      if (access_token?.value) {
        const headers = new Headers(options.headers)
        headers.set('Authorization', `Bearer ${access_token?.value}`)
        options.headers = headers
        options.credentials = 'include'
      }
    },
    async onResponseError({ response }) {
      switch (response.status) {
        case 401:
          toast({
            title: 'Unauthorized',
            description: 'Please log in again',
            variant: 'destructive'
          })
          access_token.value = null
          await nuxtApp.runWithContext(() => {
            if (confirm('Session expired. Please log in again.')) {
              useAuth().logout()
            }
          })
          break
        case 403:
          toast({
            title: 'Access Denied',
            description: 'You do not have permission to access this resource.',
            variant: 'destructive'
          })
          break
        case 404:
          toast({
            title: 'Not Found',
            description: 'The resource you are looking for does not exist.',
            variant: 'destructive'
          })
          break
        default:
          toast({
            title: 'Error',
            description: `An error occurred: ${response.statusText}`,
            variant: 'destructive'
          })
          break
      }
    }
  })
  return {
    provide: {
      api
    }
  }
})
