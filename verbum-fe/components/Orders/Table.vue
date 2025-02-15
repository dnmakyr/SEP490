<script setup lang="ts" generic="TData extends {orderId: string}, TValue">
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow
} from '@/components/ui/table'
import {
  FlexRender,
  getCoreRowModel,
  getPaginationRowModel,
  getFilteredRowModel,
  useVueTable,
  type ColumnDef,
  type ColumnFiltersState,
} from "@tanstack/vue-table"
import { valueUpdater } from '~/lib/utils';

const props = defineProps<{
  columns: ColumnDef<TData, TValue>[],
  data: TData[]
}>()

const columnFilters = ref<ColumnFiltersState>([])
const role = useAuthStore().user?.role as string | undefined
const table = useVueTable({
  get data() { return props.data },
  get columns() { return props.columns },
  getCoreRowModel: getCoreRowModel(),
  getPaginationRowModel: getPaginationRowModel(),
  onColumnFiltersChange: updaterOrValue => valueUpdater(updaterOrValue, columnFilters),
  getFilteredRowModel: getFilteredRowModel(),
  state: {
    get columnFilters() {
      return columnFilters.value
    }
  }
})
const pageIndex = ref(table.getState().pagination.pageIndex)
const pageCount = table.getPageCount()

const toPreviousPage = () => {
  table.previousPage()
  pageIndex.value = table.getState().pagination.pageIndex
}

const toNextPage = () => {
  table.nextPage()
  pageIndex.value = table.getState().pagination.pageIndex
}

const toCreate = () => {
  navigateTo("/orders/create")
}
</script>

<template>
  <div>
    <div class="flex justify-between space-x-4 pb-4">
      <div class="flex space-x-2">
        <Input 
          class="max-w-sm border border-primary" placeholder="Filter orders..."
          :model-value="table.getColumn('orderName')?.getFilterValue() as string"
          @update:model-value=" table.getColumn('orderName')?.setFilterValue($event)" />
        <Select 
          :model-value="table.getColumn('orderStatus')?.getFilterValue() as string"
          @update:model-value="table.getColumn('orderStatus')?.setFilterValue($event)">
          <SelectTrigger class="w-[20rem]">
            <SelectValue placeholder="Filter by status" />
          </SelectTrigger>
          <SelectContent>
            <SelectGroup>
              <SelectItem value="NEW">New</SelectItem>
              <SelectItem value="ACCEPTED">Accepted</SelectItem>
              <SelectItem value="REJECTED">Rejected</SelectItem>
              <SelectItem value="IN_PROGRESS">In Progress</SelectItem>
              <SelectItem value="COMPLETED">Completed</SelectItem>
              <SelectItem value="CANCELLED">Cancelled</SelectItem>
            </SelectGroup>
            <SelectSeparator />
            <Button variant="outline" @click="table.getColumn('orderStatus')?.setFilterValue('')">Clear Status
              Filter</Button>
          </SelectContent>
        </Select>
      </div>
      <Button v-if="role === 'CLIENT'" @click="toCreate">Create an Order</Button>
    </div>

    <div class="border rounded-lg overflow-hidden">
      <Table>
        <TableHeader>
          <TableRow v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
            <TableHead v-for="header in headerGroup.headers" :key="header.id">
              <FlexRender 
              v-if="!header.isPlaceholder" :render="header.column.columnDef.header"
                :props="header.getContext()" />
            </TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          <template v-if="table.getRowModel().rows?.length">
            <TableRow 
            v-for="row in table.getRowModel().rows" :key="row.id" class="cursor-pointer"
              :data-state="row.getIsSelected() ? 'selected' : undefined"
              @click="navigateTo(`/orders/details/${row.original.orderId}`)">
              <TableCell v-for="cell in row.getVisibleCells()" :key="cell.id">
                <FlexRender :render="cell.column.columnDef.cell" :props="cell.getContext()" />
              </TableCell>
            </TableRow>
          </template>
          <template v-else>
            <TableRow>
              <TableCell :colspan="columns.length" class="h-24 text-center">
                No results.
              </TableCell>
            </TableRow>
          </template>
        </TableBody>
      </Table>
    </div>
    <div class="flex items-center justify-end py-4 space-x-2">
      <Button variant="outline" size="sm" :disabled="!table.getCanPreviousPage()" @click="toPreviousPage()">
        Previous
      </Button>
      <p> {{  pageIndex + 1 }} of {{ pageCount }}</p>
      <Button variant="outline" size="sm" :disabled="!table.getCanNextPage()" @click="toNextPage()">
        Next
      </Button>
    </div>
  </div>
</template>
