import type { UseFetchOptions } from '#app'

export const useAPI = <T>(
  url: string | (() => string),
  options?: UseFetchOptions<T>
) => {
  return useFetch(url, {
    ...options,
    $fetch: useNuxtApp().$api
  })
}
