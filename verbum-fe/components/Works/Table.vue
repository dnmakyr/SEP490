<script setup lang="ts" generic="TData extends { workId: string }, TValue">
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

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

const props = defineProps<{
  columns: ColumnDef<TData, TValue>[],
  data: TData[]
}>()

const columnFilters = ref<ColumnFiltersState>([])

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
</script>

<template>
  <div>
    <div class="flex space-x-4 pb-4">
      <Input
      class="max-w-sm" placeholder="Search works..."
        :model-value="table.getColumn('workName')?.getFilterValue() as string"
        @update:model-value=" table.getColumn('workName')?.setFilterValue($event)" />
      <Select
        :model-value="table.getColumn('orderStatus')?.getFilterValue() as string"
        @update:model-value="table.getColumn('orderStatus')?.setFilterValue($event)">
        <SelectTrigger class="w-[20rem]">
          <SelectValue placeholder="Select a status" />
        </SelectTrigger>
        <SelectContent>
          <SelectGroup>
            <SelectItem value="NEW">New</SelectItem>
            <SelectItem value="IN_PROGRESS">In Progress</SelectItem>
            <SelectItem value="COMPLETED">Completed</SelectItem>
            <SelectItem value="CANCELLED">Cancelled</SelectItem>
          </SelectGroup>
          <SelectSeparator />
          <Button variant="outline" @click="table.getColumn('orderStatus')?.setFilterValue('')">Clear Status Filter</Button>
        </SelectContent>
      </Select>
    </div>
  </div>
  <div class="border rounded-md ">
    <Table>
      <TableHeader>
        <TableRow v-for="headerGroup in table.getHeaderGroups()" :key="headerGroup.id">
          <TableHead v-for="header in headerGroup.headers" :key="header.id">
            <FlexRender v-if="!header.isPlaceholder" :render="header.column.columnDef.header" />
          </TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <template v-if="table.getRowModel().rows?.length">
          <TableRow
            v-for="row in table.getRowModel().rows" :key="row.id"
            :data-state="row.getIsSelected() ? 'selected' : undefined"
          >
            <TableCell v-for="cell in row.getVisibleCells()" :key="cell.id" @click="useRouter().push(`/works/details/${row.original.workId}`)">
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
  <div>
    <div class="flex items-center justify-end py-4 space-x-2">
      <Button variant="outline" size="sm" :disabled="!table.getCanPreviousPage()" @click="table.previousPage()">
        Previous
      </Button>
      <Button variant="outline" size="sm" :disabled="!table.getCanNextPage()" @click="table.nextPage()">
        Next
      </Button>
    </div>
  </div>
</template>