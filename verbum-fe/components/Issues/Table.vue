<script setup lang="ts">
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { ref, computed, watch } from 'vue';
import type { Issue } from '~/types/issues';

import {
  Pagination,
  PaginationEllipsis,
  PaginationFirst,
  PaginationLast,
  PaginationList,
  PaginationListItem,
  PaginationNext,
  PaginationPrev,
} from '@/components/ui/pagination';
import { Button } from '@/components/ui/button';

const showIssuesDialog = ref(false);
const selectedData = ref();
const searchTerm = ref('');
const currentPage = ref(1);
const itemsPerPage = ref(10);

const emit = defineEmits(['update', 'update-status']);

const openIssuesDialog = (data: Issue) => {
  selectedData.value = data;
  showIssuesDialog.value = true;
};

const closeIssuesDialog = () => {
  selectedData.value = '';
  showIssuesDialog.value = false;
};

const props = defineProps<{
  issues: Issue[];
  role: string | undefined;
}>();

const issues = ref(props.issues);

const filteredIssues = computed(() => {
  const search = searchTerm.value.toLowerCase();
  return issues.value.filter((issue) =>
    issue.issueName.toLowerCase().includes(search)
  );
});

const paginatedIssues = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredIssues.value.slice(start, end);
});

const totalPages = computed(() =>
  Math.ceil(filteredIssues.value.length / itemsPerPage.value)
);

const goToPage = (page: number) => {
  currentPage.value = page;
};

const updateIssueInTable = (updatedIssue: Issue) => {
  emit('update', updatedIssue);
  closeIssuesDialog();
};

const updateIssueStatus = (issuesId: string, status: string) => {
  emit('update-status', issuesId, status);
};

watch(
  () => props.issues,
  (newList) => {
    issues.value = [...newList];
  },
  { deep: true }
);

</script>

<template>
  <div>
    <!-- Search Bar -->
    <div class="mb-4">
      <Input
        v-model="searchTerm"
        type="text"
        placeholder="Search by issue name"
        class="input input-bordered w-full"
      />
    </div>

    <!-- Table -->
    <Table>
      <TableHeader>
        <TableRow>
          <TableHead>Issue Name</TableHead>
          <TableHead>Order</TableHead>
          <TableHead>Created</TableHead>
          <TableHead>Updated</TableHead>
          <TableHead>Status</TableHead>
        </TableRow>
      </TableHeader>
      <TableBody>
        <TableRow
          v-for="issue in paginatedIssues"
          :key="issue.issueId"
          @click="openIssuesDialog(issue)"
        >
          <TableCell class="font-medium">
            {{ issue.issueName }}
          </TableCell>
          <TableCell class="hyper-link">
            <NuxtLink :to="`/orders/details/${issue.orderId}`">
              {{ issue.orderName }}
            </NuxtLink>
          </TableCell>
          <TableCell>
            {{
              formatDistanceToNowUserTimezone(new Date(issue.createdAt))
            }}
          </TableCell>
          <TableCell>
            {{
              formatDistanceToNowUserTimezone(new Date(issue.updatedAt))
            }}
          </TableCell>
          <TableCell>
            <Badge :class="getIssueBadgeClass(issue.status)">
              {{ issue.status }}
            </Badge>
          </TableCell>
        </TableRow>
      </TableBody>
    </Table>

    <div class="flex justify-end">
      <Pagination
      v-model="currentPage"
      :total="totalPages"
      :sibling-count="1"
      :items-per-page="itemsPerPage"
      show-edges
    >
      <PaginationList v-slot="{ items }" class="flex items-center gap-1 mt-4">
        <PaginationFirst />
        <PaginationPrev />

        <template v-for="(item, index) in items">
          <PaginationListItem
            v-if="item.type === 'page'"
            :key="index"
            :value="item.value"
            as-child
          >
            <Button
              class="w-10 h-10 p-0"
              :variant="item.value === currentPage ? 'default' : 'outline'"
              @click="goToPage(item.value)"
            >
              {{ item.value }}
            </Button>
          </PaginationListItem>
          <PaginationEllipsis v-else :key="item.type" :index="index" />
        </template>

        <PaginationNext />
        <PaginationLast />
      </PaginationList>
    </Pagination>
    </div>
  </div>

  <!-- Issues Dialog -->
  <IssuesDialog
    v-if="showIssuesDialog"
    :row-data="selectedData"
    :open="showIssuesDialog"
    :role="props.role"
    @close="closeIssuesDialog"
    @update="updateIssueInTable"
    @update-status="updateIssueStatus"
  />
</template>
