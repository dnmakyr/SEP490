/* eslint-disable @typescript-eslint/no-unused-vars */
import type { ColumnDef } from "@tanstack/vue-table";
import type { Discount } from "~/types/discount";
import Checkbox from "../ui/checkbox/Checkbox.vue";
import DropdownRowAction from "./DropdownRowAction.vue";

export const columns: ColumnDef<Discount>[] = [
  {
    id: 'select',
    header: ({ table }) => h(Checkbox, {}),
    cell: ({ row }) => h(Checkbox, {}),
    enableSorting: false,
    enableHiding: false
  },
  {
    accessorKey: 'discountId',
    header: 'ID',
    cell: ({ row }) =>
      h('div', { class: 'uppercase' }, row.getValue('discountId'))
  },
  {
    accessorKey: 'discountName',
    header: 'Name',
    cell: ({ row }) =>
      h('div', { class: 'capitalize' }, row.getValue('discountName'))
  },
  {
    accessorKey: 'discountPercent',
    header: 'Percent',
    cell: ({ row }) =>
      h('div', { class: 'capitalize' }, row.getValue('discountPercent') + '%')
  },
  {
    accessorKey: 'action',
    header: '',
    cell: ({ row }) =>
      h(DropdownRowAction, {
        rowData: row.original // Pass the row data as a prop
      })
  }
]
