<script setup lang="ts">
import { Button } from '@/components/ui/button'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '@/components/ui/dialog'
import { toTypedSchema } from '@vee-validate/zod'
import * as z from 'zod'
import type { CreateIssuePayload } from '~/types/payload/createIssue'

const { createIssue } = useIssues()

const props = defineProps({
  jobDeliverables: {
    type: Array,
    default: () => []
  },
  orderId: {
    type: String,
    default: ''
  }
})

const formSchema = toTypedSchema(
  z.object({
    issueName: z.string().min(2).max(50),
    deliverableUrl: z.string().min(1, { message: 'Required' }),
    issueDescription: z.string().min(10).max(255),
    issueAttachments: z.string()
  })
)

async function onSubmit(values: CreateIssuePayload) {
  console.log(values)

  const payload = {
    ...values,
    orderId: props.orderId,
    issueAttachments: values.issueAttachments
      ? values.issueAttachments
          .split(',')
          .map((url: string) => ({
            attachmentUrl: url.trim(),
            tag: 'ATTACHMENT'
          }))
      : []
  }

  const response = await createIssue(payload)
  if (response) {
    window.location.reload()
  }
}
</script>

<template>
  <Form
    id="dialogForm"
    v-slot="{ submitForm }"
    :validation-schema="formSchema"
    @submit="onSubmit"
  >
    <Dialog>
      <DialogTrigger as-child>
        <Button>Create Issue</Button>
      </DialogTrigger>
      <DialogContent class="max-w-[1000px] max-h-[750px] overflow-y-scroll">
        <DialogHeader>
          <DialogTitle>Create Issues</DialogTitle>
          <DialogDescription>
            Let us know what issues you are having with your order.
          </DialogDescription>
        </DialogHeader>

        <form @submit="submitForm">
          <IssuesCreateForm :job-deliverables="props.jobDeliverables" />
        </form>

        <DialogFooter>
          <DialogClose as-child>
          <Button type="button" variant="secondary">
            Cancel
          </Button>
        </DialogClose>
          <Button type="submit" form="dialogForm">Create</Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  </Form>
</template>
