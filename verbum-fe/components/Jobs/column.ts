/* eslint-disable @typescript-eslint/no-unused-vars */

import type { ColumnDef } from "@tanstack/vue-table";
import Checkbox from "../ui/checkbox/Checkbox.vue";
import type { Job } from "@/types/job";
import Badge from "../ui/badge/Badge.vue";
import { computed } from "vue";

export function getColumns(userRole: string | undefined): ColumnDef<Job>[] {
    const columns: ColumnDef<Job>[] = [
        {
            accessorKey: 'name',
            header: 'Name',
            cell: ({ row }) => {
                const name = row.getValue('name') as string
                const jobName = getJobName(name);
                return h('div', jobName)
            },
        },
        {
            accessorKey: 'targetLanguageId',
            header: 'Target Language',
            cell: ({ row }) => {
              const targetLanguageId = row.getValue('targetLanguageId') as string
              return h(
                        Badge,
                        { class: 'bg-primary text-white', variant: 'default' },
                        { default: () => getLanguageName(targetLanguageId) }
                    )
            }
          },
        {
            id: 'assigneeNames',
            header: 'Assignees',
            cell: ({ row }) => {
                const assignees = row.original.assigneeNames || []; // Assuming `row.original.assignees` contains the array
                const names = assignees.map((assignee: { name: string }) => assignee.name).join(', ');
                return h('div', { class: 'truncate', title: names }, names || 'No Assignees');
            },
        },
        {
            accessorKey: 'status',
            header: 'Status',
            cell: ({ row }) => {
              const status = row.getValue('status') as string
              return h(
                Badge,
                { class: getJobBadgeClass(status), variant: 'default' },
                { default: () => status }
              )
            }
          }
    ];

    return userRole === 'Linguist'
    ? columns.filter(col => col.id !== 'assigneeNames')
    : columns;
}