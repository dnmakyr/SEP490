<script lang="ts" setup>
import { ref, watch, onMounted, defineEmits, computed } from 'vue'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  Dialog,
  DialogContent,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogDescription
} from '@/components/ui/dialog'
import {
  Table,
  TableHeader,
  TableRow,
  TableCell,
  TableBody
} from '@/components/ui/table'
import type { Issue, IssueAttachments } from '~/types/issues'
import { formatDate } from '~/utils/date'
import { useUsers } from '~/composables/useUsers'
import { getIssueBadgeClass } from '@/utils/getBadgeClass'
import { getFirebaseFileName } from '~/utils/getFirebaseFileName'
import type { ResolveIssuePayload } from '~/types/payload/resolveIssue'
import { useFileDialog } from '@vueuse/core'
import { cn } from '@/lib/utils'
import {
  ref as storageRef,
  getDownloadURL,
  uploadBytesResumable
} from 'firebase/storage'
import { ServiceManagersRole } from '~/constants/userRole'

const { assignList, getAssignList } = useUsers()
const {
  updateIssueStatus,
  sendCancelResponse,
  updateIssue,
  resolveIssue,
  approveIssueSolution,
  acceptIssueSolution,
  sendRejectResponse,
  reOpenIssue
} = useIssues()

const props = defineProps<{
  open: boolean
  rowData: Issue
  role: string
}>()

const emit = defineEmits(['close', 'update', 'update-status'])
const isOpen = ref(props.open)
const isEditing = ref(false)
const isStatusEditing = ref(false)
const previousStatus = ref('')
const titleStatusConfirm = 'Change status'
const descriptionStatusConfirm = 'Are you really want to cancel this issue?'
const rejectSolutionDescription =
  'Are you sure you want to reject this solution, the Status of this issue will change to In Progress.'
const descriptionStatusReOpen =
  'Are you really want to re-open the issue on this file? You can provides us new issue information after clicking "Confirm"'
const isConfirmDialogOpen = ref(false)
const isCancelDialogOpen = ref(false)
const isResolveDialogOpen = ref(false)
const isUploadAttachmentDialogOpen = ref(false)
const isRejectSolutionDialogOpen = ref(false)
const isReOpenDialogOpen = ref(false)
const reasonForCancellation = ref('')
const reasonForRejectSolution = ref('')

const storage = useFirebaseStorage()
const downloadUrls = ref<string[]>([])
const uploadProgress = ref<number[]>([])

const downloadUrlsString = computed(() => downloadUrls.value.join(','))

const { files, open: openFileSelect } = useFileDialog()

async function uploadFiles() {
  if (files.value?.length) {
    downloadUrls.value = []
    const promises = Array.from(files.value).map(
      (file, index) =>
        new Promise<string>((resolve, reject) => {
          const fileRef = storageRef(storage, `uploads/${file.name}`)
          const uploadTask = uploadBytesResumable(fileRef, file)

          uploadTask.on(
            'state_changed',
            (snapshot) => {
              const progress =
                (snapshot.bytesTransferred / snapshot.totalBytes) * 100
              uploadProgress.value[index] = Math.round(progress)
            },
            (error) => {
              reject(error)
            },
            async () => {
              const url = await getDownloadURL(fileRef)
              resolve(url)
            }
          )
        })
    )

    const urls = await Promise.all(promises)
    downloadUrls.value = urls
  }
}

watch(files, () => {
  if (files.value?.length) {
    uploadProgress.value = Array(files.value.length).fill(0)
    uploadFiles()
  }
})

onMounted(() => {
  if (!assignList.value.length) {
    getAssignList()
  }
})

async function refreshPage() {
  window.location.reload()
}

const issue = ref(props.rowData)
const issueStatuses = ['OPEN', 'IN_PROGRESS', 'CANCEL', 'SUBMITTED', 'RESOLVED']
const selectedStatus = ref(issue.value.status)

const updateIssueDetail = async () => {
  const issueAttachment: IssueAttachments[] = [
    {
      attachmentUrl: downloadUrlsString.value,
      tag: 'ATTACHMENT'
    }
  ]

  const payload = {
    issueId: issue.value.issueId,
    issueName: issue.value.issueName,
    issueDescription: issue.value.issueDescription,
    assigneeId: issue.value.assigneeId,
    issueAttachments:
      issueAttachment[0].attachmentUrl.length > 0
        ? issueAttachment
        : issue.value.issueAttachments
  }
  await updateIssue(payload)
  if (
    issue.value.status === 'OPEN' &&
    ServiceManagersRole.includes(props.role)
  ) {
    await updateIssueStatus(issue.value.issueId, 'IN_PROGRESS')
  }
  refreshPage()
  isEditing.value = false
}

const enableEditing = () => {
  isEditing.value = true
}

const getUserIdByName = (users: User[], name: string): string => {
  const user = users.find((user) => user.name === name)
  return user?.id ?? ''
}

const handleResolveIssue = async () => {
  const solutionAttachment: IssueAttachments = {
    attachmentUrl: downloadUrlsString.value,
    tag: 'SOLUTION'
  }

  const updatedIssueAttachments = [solutionAttachment]

  const payload: ResolveIssuePayload = {
    issueId: issue.value.issueId,
    issueName: issue.value.issueName,
    issueDescription: issue.value.issueDescription,
    assigneeId: getUserIdByName(assignList.value, issue.value.assigneeName),
    issueAttachments: updatedIssueAttachments
  }

  await resolveIssue(payload)
  await updateIssueStatus(issue.value.issueId, 'SUBMITTED')
  isResolveDialogOpen.value = false
  refreshPage()
}

const handleCancelResolve = () => {
  selectedStatus.value = previousStatus.value
  isResolveDialogOpen.value = false
}

const openRejectDialog = () => {
  isRejectSolutionDialogOpen.value = true
}

const handleConfirmStatus = async () => {
  await sendCancelResponse(issue.value.issueId, reasonForCancellation.value)
  await updateIssueStatus(issue.value.issueId, 'CANCEL')
  isCancelDialogOpen.value = false
  refreshPage()
}

const handleConfirmReOpen = async () => {
  issue.value.status = 'OPEN'
  await reOpenIssue(issue.value)
  isCancelDialogOpen.value = false
  await refreshPage()
}

const handleCancelReOpen = () => {
  selectedStatus.value = previousStatus.value
  isReOpenDialogOpen.value = false
}

const handleCancelStatus = () => {
  selectedStatus.value = previousStatus.value
  isCancelDialogOpen.value = false
  reasonForCancellation.value = ''
}

const handleConfirmRejectSolution = async () => {
  await sendRejectResponse(issue.value.issueId, reasonForRejectSolution.value)
  await updateIssueStatus(issue.value.issueId, 'IN_PROGRESS')
  isCancelDialogOpen.value = false
  refreshPage()
}

const handleCancelRejectSolution = () => {
  selectedStatus.value = previousStatus.value
  isRejectSolutionDialogOpen.value = false
  reasonForRejectSolution.value = ''
}

const handleUploadIssueAttachment = () => {
  selectedStatus.value = previousStatus.value
  isUploadAttachmentDialogOpen.value = true
  reasonForRejectSolution.value = ''
}

const handleStatusChange = async (
  issuesId: string,
  oldStatus: string,
  newStatus: string
) => {
  if (newStatus === 'CANCEL') {
    isCancelDialogOpen.value = true
    previousStatus.value = oldStatus
  } else if (newStatus === 'SUBMITTED') {
    isResolveDialogOpen.value = true
    previousStatus.value = oldStatus
  } else if (newStatus === 'OPEN') {
    isReOpenDialogOpen.value = true
    previousStatus.value = oldStatus
  } else {
    await updateIssueStatus(issuesId, newStatus)
    issue.value.status = newStatus
  }
}

const handleReviewResolveIssue = async () => {
  const response = await approveIssueSolution(issue.value.issueId)
  if (response) {
    await acceptIssueSolution(issue.value.issueId)
  }
  await refreshPage()
}

const closeDialog = () => {
  emit('close')
  isEditing.value = false
  isOpen.value = false
}

// Compute allowed statuses based on the role
const filteredIssueStatuses = computed(() => {
  switch (props.role) {
    case 'CLIENT':
      return ['CANCEL', 'OPEN']
    case 'EDIT_MANAGER':
    case 'EVALUATE_MANAGER':
    case 'TRANSLATE_MANAGER':
      return ['CANCEL']
    case 'LINGUIST':
      return ['SUBMITTED']
    default:
      return issueStatuses
  }
})

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
    if (!newVal) {
      isEditing.value = false
      isStatusEditing.value = false
    }
  }
)

const solutionlist = issue.value.issueAttachments.filter(
  (attachment) => attachment.tag === 'SOLUTION'
)
</script>

<template>
  <Dialog :open="isOpen">
    <DialogContent class="max-w-[1000px] max-h-[750px]">
      <DialogHeader>
        <DialogTitle class="font-semibold text-4xl text-cyan-700">
          {{ issue.issueName }}
        </DialogTitle>
      </DialogHeader>

      <div class="flex gap-3 h-max overflow-scroll">
        <div class="flex-1 flex flex-col gap-3">
          <div
            v-if="issue.cancelResponse"
            class="p-3 rounded-xl border-2 border-stone-300"
          >
            <div class="font-semibold text-red-600">Cancellation Reason:</div>
            <p>{{ issue.cancelResponse }}</p>
          </div>

          <div
            v-if="
              issue.rejectResponse &&
              (role === 'LINGUIST' || ServiceManagersRole.includes(role)) &&
              issue.status !== 'RESOLVED'
            "
            class="p-3 rounded-xl border-2 border-stone-300"
          >
            <div class="font-semibold text-red-600">
              Reject Solution Reason:
            </div>
            <p>{{ issue.rejectResponse }}</p>
          </div>

          <!-- Description -->
          <div class="p-3 rounded-xl border-2 border-stone-300">
            <div class="font-semibold">Description:</div>
            <p>
              <template v-if="isEditing && role === 'CLIENT'">
                <Textarea
                  v-model="issue.issueDescription"
                  class="border border-cyan-700 rounded p-1 w-full"
                />
              </template>
              <template v-else>{{ issue.issueDescription }}</template>
            </p>
          </div>

          <div class="p-3 rounded-xl border-2 border-stone-300">
            <div class="font-semibold">File Having Issue:</div>
            <div v-if="issue.documentUrl !== 0" class="flex gap-3">
              <div>
                <a
                  :href="issue.documentUrl"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="border rounded-xl flex flex-col gap-3 w-[150px] justify-center items-center p-2 hover:bg-stone-200"
                  :title="getFirebaseFileName(issue.documentUrl)"
                >
                  <img
                    src="~/assets/img/file_icon.png"
                    loading="eager"
                    format="avif"
                    width="50"
                    height="50"
                    alt="file icon"
                  />
                  <h1
                    class="whitespace-nowrap overflow-hidden text-ellipsis w-full text-center px-2"
                  >
                    {{ getFirebaseFileName(issue.documentUrl) }}
                  </h1>
                </a>
              </div>
            </div>
            <div v-else>
              <p class="text-primary font-semibold">No attachments found</p>
            </div>
          </div>

          <!-- Issue Attachment -->
          <div class="p-3 rounded-xl border-2 border-stone-300">
            <div class="font-semibold">Issue Attachment Files:</div>
            <div v-if="issue.issueAttachments.length !== 0" class="flex gap-3">
              <div
                v-for="attachment in issue.issueAttachments"
                :key="attachment.attachmentUrl"
              >
                <a
                  v-if="attachment.tag === 'ATTACHMENT'"
                  :href="attachment.attachmentUrl"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="border rounded-xl flex flex-col gap-3 w-[150px] justify-center items-center p-2 hover:bg-stone-200"
                  :title="getFirebaseFileName(attachment.attachmentUrl)"
                >
                  <img
                    src="~/assets/img/file_icon.png"
                    loading="eager"
                    format="avif"
                    width="50"
                    height="50"
                    alt="file icon"
                  />
                  <h1
                    class="whitespace-nowrap overflow-hidden text-ellipsis w-full text-center px-2"
                  >
                    {{ getFirebaseFileName(attachment.attachmentUrl) }}
                  </h1>
                </a>
              </div>
            </div>
            <div v-else>
              <p class="text-primary font-semibold">No attachments found</p>
            </div>
            <Button v-if="isEditing && role === 'CLIENT'" @click="handleUploadIssueAttachment"
              >Upload Issue Attachment</Button
            >
          </div>
          
          <!-- Solution file list -->
          <div class="p-3 rounded-xl border-2 border-stone-300">
            <div class="flex justify-between">
              <div class="font-semibold">Solution Files:</div>
              <div
                v-if="
                  ServiceManagersRole.includes(role) &&
                  issue.status === 'SUBMITTED' &&
                  solutionlist.length !== 0
                "
                class="flex gap-2"
              >
                <Button class="bg-gray-500" @click="openRejectDialog"
                  >Reject</Button
                >
                <Button class="bg-red-500" @click="handleReviewResolveIssue"
                  >Mark as resolved</Button
                >
              </div>
            </div>
            <div v-if="solutionlist.length !== 0" class="flex gap-3">
              <div
                v-for="attachment in solutionlist"
                :key="attachment.attachmentUrl"
              >
                <a
                  :href="attachment.attachmentUrl"
                  target="_blank"
                  rel="noopener noreferrer"
                  class="border rounded-xl flex flex-col gap-3 w-[150px] justify-center items-center p-2 hover:bg-stone-200"
                  :title="getFirebaseFileName(attachment.attachmentUrl)"
                >
                  <img
                    src="~/assets/img/file_icon.png"
                    loading="eager"
                    format="avif"
                    width="50"
                    height="50"
                    alt="file icon"
                  />
                  <h1
                    class="whitespace-nowrap overflow-hidden text-ellipsis w-full text-center px-2"
                  >
                    {{ getFirebaseFileName(attachment.attachmentUrl) }}
                  </h1>
                </a>
              </div>
            </div>
            <div v-else>
              <p class="text-primary font-semibold">No solution yet</p>
            </div>
            <Button
              v-if="role === 'LINGUIST' && issue.status === 'IN_PROGRESS'"
              @click="isResolveDialogOpen = true"
            >
              Upload Solution
            </Button>
          </div>
        </div>

        <div class="p-3 w-1/3 rounded-xl border-2 border-stone-300">
          <Table>
            <TableHeader>
              <TableRow>
                <TableCell class="font-semibold">Issue name:</TableCell>
                <TableCell>
                  <template v-if="isEditing && role === 'CLIENT'">
                    <Input
                      v-model="issue.issueName"
                      class="rounded p-1 w-full"
                    />
                  </template>
                  <template v-else>{{ issue.issueName }}</template>
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell class="font-semibold">Order:</TableCell>
                <TableCell>
                  <NuxtLink
                    :to="`/orders/details/${issue.orderId}`"
                    class="hyper-link"
                  >
                    {{ issue.orderName }}
                  </NuxtLink>
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell class="font-semibold">File:</TableCell>
                <TableCell
                  ><a
                    :href="issue.documentUrl"
                    class="hyper-link whitespace-nowrap overflow-hidden text-ellipsis"
                    >{{ getFirebaseFileName(issue.documentUrl) }}</a
                  ></TableCell
                >
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-if="role !== 'CLIENT'">
                <TableCell class="font-semibold">Created by:</TableCell>
                <TableCell>
                  {{ issue.clientName }}
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell class="font-semibold">Created date:</TableCell>
                <TableCell>{{ formatDate(issue.createdAt) }}</TableCell>
              </TableRow>
              <TableRow>
                <TableCell class="font-semibold">Updated date:</TableCell>
                <TableCell>{{ formatDate(issue.updatedAt) }}</TableCell>
              </TableRow>
              <TableRow v-if="role !== 'CLIENT'">
                <TableCell class="font-semibold">Assign:</TableCell>
                <TableCell>
                  <template
                    v-if="
                      isEditing &&
                      ServiceManagersRole.includes(role) &&
                      ['OPEN', 'IN_PROGRESS'].includes(issue.status)
                    "
                  >
                    <Select
                      v-model="issue.assigneeId"
                      :value="issue.assigneeId"
                      class="border border-cyan-700 rounded w-full"
                      @update:modelValue="
                        (newAssigneeId) =>
                          (issue.assigneeName =
                            assignList.find((user) => user.id === newAssigneeId)
                              ?.name ?? '')
                      "
                    >
                      <SelectTrigger class="w-[180px]">
                        <SelectValue :placeholder="issue.assigneeName" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectGroup>
                          <SelectItem
                            v-for="user in assignList"
                            :key="user.id"
                            :value="user.id"
                          >
                            {{ user.name }}
                          </SelectItem>
                        </SelectGroup>
                      </SelectContent>
                    </Select>
                  </template>
                  <template v-else>{{ issue.assigneeName }}</template>
                </TableCell>
              </TableRow>
              <TableRow>
                <TableCell class="font-semibold">Status:</TableCell>
                <TableCell
                  ><Select
                    v-model="selectedStatus"
                    @update:modelValue="
                      (newStatus) =>
                        handleStatusChange(
                          issue.issueId,
                          issue.status,
                          newStatus
                        )
                    "
                  >
                    <SelectTrigger
                      class="max-w-fit p-0 border-none focus:ring-0 focus:ring-offset-0 [&_svg]:hidden"
                    >
                      <SelectValue>
                        <Badge :class="getIssueBadgeClass(selectedStatus)"
                          >{{ selectedStatus }}
                        </Badge>
                      </SelectValue>
                    </SelectTrigger>
                    <SelectContent v-if="filteredIssueStatuses.length !== 0">
                      <SelectGroup>
                        <SelectItem
                          v-for="issueStatus in filteredIssueStatuses.filter(
                            (issue) =>
                              selectedStatus === 'RESOLVED'
                                ? issue === 'OPEN'
                                : issue !== selectedStatus
                          )"
                          :key="issueStatus"
                          :value="issueStatus"
                        >
                          <Badge :class="getIssueBadgeClass(issueStatus)"
                            >{{ issueStatus }}
                          </Badge>
                        </SelectItem>
                      </SelectGroup>
                    </SelectContent>
                  </Select></TableCell
                >
              </TableRow>
            </TableBody>
          </Table>
        </div>
      </div>

      <DialogFooter>
        <Button v-if="isEditing" class="bg-slate-500" @click="closeDialog"
          >Cancel
        </Button>
        <Button v-if="!isEditing" class="bg-slate-500" @click="closeDialog"
          >Close
        </Button>
        <Button
          v-if="
            !isEditing &&
            ((role === 'CLIENT' && issue.status == 'OPEN') ||
              (ServiceManagersRole.includes(role) &&
                issue.status == 'IN_PROGRESS'))
          "
          @click="enableEditing"
          >Edit
        </Button>
        <Button
          v-if="ServiceManagersRole.includes(role) && issue.status == 'OPEN'"
          @click="updateIssueDetail"
          >Accept Issue
        </Button>
        <Button v-if="isEditing" @click="updateIssueDetail">Update</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
  <IssuesConfirmDialog
    :title="titleStatusConfirm"
    :description="descriptionStatusConfirm"
    :open="isConfirmDialogOpen"
    @close="handleCancelStatus"
    @confirm="handleConfirmStatus"
  />

  <Dialog :open="isCancelDialogOpen" @close="handleCancelStatus">
    <DialogContent class="max-w-md">
      <DialogHeader>
        <DialogTitle>Provide Cancellation Reason</DialogTitle>
      </DialogHeader>
      <Input
        v-model="reasonForCancellation"
        placeholder="Enter reason for cancellation"
        class="w-full"
      />
      <DialogDescription class="text-red-500 font-semibold">
        {{ descriptionStatusConfirm }}
      </DialogDescription>
      <DialogFooter>
        <Button class="bg-gray-500" @click="handleCancelStatus">Cancel</Button>
        <Button class="bg-red-500" @click="handleConfirmStatus">Confirm</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>

  <Dialog :open="isRejectSolutionDialogOpen">
    <DialogContent class="max-w-md">
      <DialogHeader>
        <DialogTitle>Provide Reject Reason</DialogTitle>
      </DialogHeader>
      <Input
        v-model="reasonForRejectSolution"
        placeholder="Enter reason for cancellation"
        class="w-full"
      />
      <DialogDescription class="text-red-500 font-semibold">
        {{ rejectSolutionDescription }}
      </DialogDescription>
      <DialogFooter>
        <Button class="bg-gray-500" @click="handleCancelRejectSolution"
          >Cancel</Button
        >
        <Button class="bg-red-500" @click="handleConfirmRejectSolution"
          >Confirm</Button
        >
      </DialogFooter>
    </DialogContent>
  </Dialog>

  <Dialog :open="isResolveDialogOpen" @close="handleCancelResolve">
    <DialogContent class="max-w-md">
      <DialogHeader>
        <DialogTitle>Upload Issues Solution</DialogTitle>
      </DialogHeader>
      <Button
        class="block"
        type="button"
        @click="openFileSelect({ accept: '*', multiple: true })"
      >
        Upload Files
      </Button>
      <Card v-if="files?.length" :class="cn($attrs.class ?? '')">
        <CardHeader>
          <CardDescription>Uploaded files</CardDescription>
        </CardHeader>
        <CardContent class="grid gap-3">
          <div
            v-for="(file, index) in files"
            :key="file.name"
            class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
          >
            <span class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500" />
            <div class="flex flex-col gap-1">
              <p class="text-sm font-medium leading-none">
                {{ file.name }}
              </p>
              <div class="flex gap-5 max-w-sm">
                <Progress v-model="uploadProgress[index]" />
                <p class="text-sm font-medium leading-none">
                  {{ uploadProgress[index] || 0 }}%
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
      <DialogFooter>
        <Button class="bg-gray-500" @click="handleCancelResolve">Cancel</Button>
        <Button class="bg-red-500" @click="handleResolveIssue">Submit</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>

  <Dialog :open="isUploadAttachmentDialogOpen">
    <DialogContent class="max-w-md">
      <DialogHeader>
        <DialogTitle>Upload Issues Attachment</DialogTitle>
      </DialogHeader>
      <Button
        class="block"
        type="button"
        @click="openFileSelect({ accept: '*', multiple: true })"
      >
        Upload Files
      </Button>
      <Card v-if="files?.length" :class="cn($attrs.class ?? '')">
        <CardHeader>
          <CardDescription>Uploaded files</CardDescription>
        </CardHeader>
        <CardContent class="grid gap-3">
          <div
            v-for="(file, index) in files"
            :key="file.name"
            class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
          >
            <span class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500" />
            <div class="flex flex-col gap-1">
              <p class="text-sm font-medium leading-none">
                {{ file.name }}
              </p>
              <div class="flex gap-5 max-w-sm">
                <Progress v-model="uploadProgress[index]" />
                <p class="text-sm font-medium leading-none">
                  {{ uploadProgress[index] || 0 }}%
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
      <DialogFooter>
        <Button class="bg-gray-500" @click="handleCancelResolve">Cancel</Button>
        <Button class="bg-red-500" @click="updateIssueDetail">Submit</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>

  <Dialog :open="isReOpenDialogOpen">
    <DialogContent class="max-w-md">
      <DialogHeader>
        <DialogTitle>Re-open issue</DialogTitle>
      </DialogHeader>
      <DialogDescription class="text-red-500 font-semibold">
        {{ descriptionStatusReOpen }}
      </DialogDescription>
      <DialogFooter>
        <Button class="bg-gray-500" @click="handleCancelReOpen">Cancel</Button>
        <Button class="bg-red-500" @click="handleConfirmReOpen">Confirm</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
