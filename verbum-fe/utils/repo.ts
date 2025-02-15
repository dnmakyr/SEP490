import type { $Fetch, NitroFetchRequest } from 'nitropack'
import type { Order } from '@/types/order'
import type { Language } from '@/types/language'
import type { Job } from '@/types/job'
export const repo = <T>(fetch: $Fetch<T, NitroFetchRequest>) => ({
  async getLanguages(): Promise<Language[]> {
    return fetch<Language[]>(`/lang`)
  },
  async getOrders(): Promise<Order[]> {
    return fetch<Order[]>(`/order/get-all?$orderby=createdDate desc`)
  },
  async getOrdersCount(): Promise<Order[]> {
    return fetch<Order[]>(`/order/get-all`)
  },
  async searchOrders(value: string): Promise<Order[]> {
    return fetch<Order[]>(`/order/get-all?$filter=contains(orderName, '${value}')`)
  },
  async updateOrder(order: Partial<Order>): Promise<Order | null> {
    return fetch<Order | null>(`/order/update`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(order)
    })
  },
  async assignLinguists(payload: Partial<Job>): Promise<Partial<Job>> {
    return fetch<Partial<Job>>(`/job/edit`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(payload)
    })
  }
})

