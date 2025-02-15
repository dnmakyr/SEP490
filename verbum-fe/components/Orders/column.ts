import type { ColumnDef } from '@tanstack/vue-table'
import type { Order } from '@/types/order'
import { formatToVietnamTimezone } from '~/utils/date'
import Badge from '../ui/badge/Badge.vue'

export const columns: ColumnDef<Order>[] = [
  {
    header: '#',
    cell: ({ row }) => h('div', {}, row.index + 1)
  },
  {
    accessorKey: 'orderName',
    header: 'Name',
    cell: ({ row }) =>
      h('div', { class: 'capitalize hyper-link' }, row.getValue('orderName'))
  },
  {
    accessorKey: 'createdDate',
    header: 'Created At',
    cell: ({ row }) => {
      const date = row.getValue('createdDate') as string
      const formattedDate = formatToVietnamTimezone(date)
      return h('div', {}, formattedDate)
    }
  },
  {
    accessorKey: 'dueDate',
    header: 'Due Date',
    cell: ({ row }) => {
      const date = row.getValue('dueDate') as string
      const formattedDate = formatToVietnamTimezone(date)
      return h('div', {}, formattedDate)
    }
  },
  {
    accessorKey: 'sourceLanguageId',
    header: 'Source Language',
    cell: ({ row }) => {
      const sourceLanguageId = row.getValue('sourceLanguageId') as string
      return h(
        Badge,
        { class: 'bg-primary text-white', variant: 'default' },
        { default: () => getLanguageName(sourceLanguageId) }
      )
    }
  },
  {
    accessorKey: 'targetLanguageId',
    header: 'Target Language',
    cell: ({ row }) => {
      const targetLanguageIds = row.getValue('targetLanguageId') as string[]
      return targetLanguageIds.map((id) =>
        h(
          Badge,
          { class: 'bg-gray-500 text-white mx-1', variant: 'default' },
          { default: () => getLanguageName(id) }
        )
      )
    }
  },
  {
    accessorKey: 'orderPrice',
    header: 'Price (USD)',
    cell: ({ row }) => {
      const orderPrice = row.getValue('orderPrice') as string
      return orderPrice ? h('div', { class: 'font-semibold text-primary' }, `$ ${orderPrice}`) : h('div', { class: 'font-semibold text-red-500' }, 'Unset')
    }
  },
  {
    accessorKey: 'orderStatus',
    header: 'Status',
    cell: ({ row }) => {
      const orderStatus = row.getValue('orderStatus') as string
      return h(
        Badge,
        { class: getOrderBadgeClass(orderStatus), variant: 'default' },
        { default: () => orderStatus }
      )
    }
  }
]
