/* eslint-disable @typescript-eslint/no-unused-vars */

import type { ColumnDef } from '@tanstack/vue-table'
import Checkbox from '../ui/checkbox/Checkbox.vue'
import type { Work } from '~/composables/useWorks'
import Badge from '../ui/badge/Badge.vue'

export const columns: ColumnDef<Work>[] = [
  {
    id: 'select',
    header: ({ table }) => h(Checkbox, {}),
    cell: ({ row }) => h(Checkbox, {}),
    enableSorting: false,
    enableHiding: false
  },
  {
    accessorKey: 'workName',
    header: 'Work Name',
    cell: ({ row }) =>
      h('div', { class: 'capitalize' }, row.getValue('workName'))
  },
  {
    accessorKey: 'serviceCode',
    header: 'Services',
    cell: ({ row }) =>
      h(
        'div',
        { class: 'capitalize font-bold text-primary' },
        getServiceName(row.getValue('serviceCode'))
      )
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
    accessorKey: 'orderStatus',
    header: 'Status',
    cell: ({ row }) => {
      let orderStatus = row.getValue('orderStatus') as string
      const isCompleted = row.original.isCompleted as boolean
      if (isCompleted && orderStatus === 'IN_PROGRESS') {
        orderStatus = 'COMPLETED'
      }
      return h(
        Badge,
        { class: getOrderBadgeClass(orderStatus), variant: 'default' },
        { default: () => orderStatus }
      );
    }
  }
  
]
