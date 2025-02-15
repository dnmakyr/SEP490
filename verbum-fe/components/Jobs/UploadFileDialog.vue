<template>
  <div>
    <Dialog>
      <DialogTrigger as-child>
        <slot />
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Upload deliverable</DialogTitle>
          <DialogDescription>
            This action cannot be undone. Check the file before submitting.
          </DialogDescription>
        </DialogHeader>
        <h1>Uploaded file</h1>
        <div v-for="file in files" :key="file.name"
          class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0">
          <span class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500" />
          <div class="flex flex-col gap-1">
            <p class="text-sm font-medium leading-none">
              {{ file.name }}
            </p>
            <div class="flex gap-5 max-w-sm">
              <Progress v-model="uploadProgress" />
              <p class="text-sm font-medium leading-none">
                {{ uploadProgress || 0 }}%
              </p>
            </div>
          </div>
        </div>
        <DialogFooter>
          <Button @click="open()" :disabled="isUploading">Select File</Button>
          <Button @click="uploadDeliverable" :disabled="isUploading || !downloadUrl">
            Submit
          </Button>
          <DialogClose as-child>
            <Button variant="outline" :disabled="isUploading">Cancel</Button>
          </DialogClose>
        </DialogFooter>
      </DialogContent>
    </Dialog>

  </div>
</template>

<script lang="ts" setup>
import type { Job } from '@/types/job'
import { useFileDialog } from '@vueuse/core'
import { ref as storageRef, getDownloadURL, uploadBytesResumable } from 'firebase/storage'
import { cn } from '@/lib/utils'
import { toast } from '../ui/toast';
const props = defineProps<{
  job: Job | undefined
}>()

const storage = useFirebaseStorage()
const downloadUrl = ref<string>('')
const isUploading = ref(false)
const uploadError = ref<string | null>(null)
const uploadProgress = ref<number>(0)
const { files, open } = useFileDialog()

async function uploadFile() {
  if (files.value?.[0]) {
    try {
      isUploading.value = true
      uploadError.value = null
      const file = files.value[0]
      const fileRef = storageRef(storage, `uploads/${file.name}`)

      const uploadTask = uploadBytesResumable(fileRef, file)

      uploadTask.on(
        'state_changed',
        (snapshot) => {
          const progress = (snapshot.bytesTransferred / snapshot.totalBytes) * 100
          uploadProgress.value = Math.round(progress)
        },
        (error) => {
          throw error
        }
      )

      await uploadTask
      const url = await getDownloadURL(fileRef)
      downloadUrl.value = url
    } catch (error: any) {
      console.error('Upload error:', error)
      uploadError.value = error.message || 'Failed to upload file'
    } finally {
      isUploading.value = false
    }
  }
}
watch(files, () => {
  if (files.value?.length) {
    uploadFile()
  }
})
const uploadDeliverable = async () => {
  const payload = {
    ...props.job,
    status: "SUBMITTED",
    assigneesId: props.job?.assigneeNames.map((assignee) => assignee.id),
    deliverableUrl: downloadUrl.value
  }
  console.log(JSON.stringify(payload, null, 2))
  try {
    const {status, error} = await useAPI('/job/edit', {
      method: 'PUT', headers: {
      'Content-Type': 'application/json'
    }, body: JSON.stringify(payload)
  })
    if (status.value === "error") {
      toast({
        title: 'Failed to upload deliverable',
        description: error.value?.message,
        variant: 'destructive'
      })
      return
    }
    if (status.value === "success") {
      toast({
        title: 'Deliverable uploaded successfully',
        description: 'The deliverable has been uploaded successfully',
      })
      window.location.reload()
    }
  } catch (error: any) {
    console.error('Error uploading deliverable:', error)
  }
}
</script>

<style></style>