<script setup>
import { onMounted } from 'vue'
import { ORDER_COMPLETED, ORDER_IN_PROGRESS } from '~/constants/orderSatus'

const { issues, getIssuesByOrders, updateIssue } = useIssues()

const props = defineProps({
  jobDeliverables: {
    type: Array,
    default: () => {}
  },
  orderId: {
    type: String,
    default: ''
  },
  role: {
    type: String,
    default: ''
  },
  user: {
    type: Object,
    default: () => ({})
  },
  orderStatus: {
    type: String,
    default: ''
  }
})

const fetchIssues = async () => {
  issues.value = await getIssuesByOrders(props.orderId)
}

const handleUpdate = async (updateIssues) => {
  await updateIssue(updateIssues)
  window.location.reload()
}

onMounted(() => {
  fetchIssues()
})
</script>

<template>
  <div
    v-if="
      props.orderStatus === ORDER_COMPLETED ||
      (props.orderStatus === ORDER_IN_PROGRESS && issues.length !== 0)
    "
    class="flex-1 space-y-4 border rounded-md"
  >
    <div class="h-full">
      <div class="flex justify-between items-center p-3 border-b">
        <span class="text-lg font-semibold text-primary">Issues</span>
        <IssuesCreate
          v-if="props.role === 'CLIENT'"
          :order-id="props.orderId"
          :job-deliverables="jobDeliverables"
        />
      </div>
      <div v-if="issues.length !== 0" class="h-[15rem] overflow-auto p-2">
        <IssuesTable
          :issues="issues"
          :role="props.role"
          @update="handleUpdate"
        />
      </div>
      <div v-else class="w-full h-full flex justify-center items-center">
        <p v-if="props.role === 'CLIENT'" class="font-bold">
          Have issues with the order?
          <span class="text-primary">Let us know.</span>
        </p>
        <p v-else class="font-bold">
          <span class="text-primary">No issue found</span>
        </p>
      </div>
    </div>
  </div>
</template>
