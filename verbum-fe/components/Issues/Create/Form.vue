<script setup lang="ts">
import {
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage
} from '@/components/ui/form'
import { Progress } from '@/components/ui/progress'
import { Input } from '@/components/ui/input'
import { Textarea } from '@/components/ui/textarea'
import { ref, computed, watch } from 'vue'
import { useFileDialog } from '@vueuse/core'
import { cn } from '@/lib/utils'
import {
  ref as storageRef,
  getDownloadURL,
  uploadBytesResumable
} from 'firebase/storage'
import type { JobDeliverables } from '~/types/jobDeliverables'
import { getFirebaseFileName } from '~/utils/getFirebaseFileName'

const props = defineProps({
  jobDeliverables: {
    type: Array<JobDeliverables>,
    default: () => []
  }
})

const storage = useFirebaseStorage()
const downloadUrls = ref<string[]>([])
const uploadProgress = ref<number[]>([]) // Track progress of each file

const downloadUrlsString = computed(() => downloadUrls.value.join(','))

async function uploadFiles() {
  if (files.value?.length) {
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
    downloadUrls.value = [...downloadUrls.value, ...urls]
  }
}

const { files, open } = useFileDialog()

watch(files, () => {
  if (files.value?.length) {
    uploadProgress.value = Array(files.value.length).fill(0)
    uploadFiles()
  }
})

const getElementsWithHighestServiceOrder = (
  jobDeliverables: JobDeliverables[]
): JobDeliverables[] => {
  let maxServiceOrder = -Infinity
  const result: JobDeliverables[] = []

  for (const item of jobDeliverables) {
    if (item.serviceOrder > maxServiceOrder) {
      maxServiceOrder = item.serviceOrder
      result.length = 0
      result.push(item)
    } else if (item.serviceOrder === maxServiceOrder) {
      result.push(item)
    }
  }

  return result
}
</script>

<template>
  <FormField v-slot="{ componentField }" name="issueName">
    <FormItem>
      <FormLabel>Title</FormLabel>
      <FormControl>
        <Input
          type="text"
          placeholder="Give your issue a title."
          v-bind="componentField"
        />
      </FormControl>
      <FormMessage />
    </FormItem>
  </FormField>

  <FormField v-slot="{ componentField }" name="deliverableUrl">
    <FormItem>
      <FormLabel>File having issues</FormLabel>
      <Select v-bind="componentField">
        <FormControl>
          <SelectTrigger>
            <SelectValue placeholder="Select a file that having issues" />
          </SelectTrigger>
        </FormControl>
        <SelectContent>
          <SelectGroup>
            <SelectItem
              v-for="item in getElementsWithHighestServiceOrder(
                props.jobDeliverables
              )"
              :key="item.deliverableFileUrl"
              :value="String(item.deliverableFileUrl)"
            >
              {{ getFirebaseFileName(item.deliverableFileUrl) }}
            </SelectItem>
          </SelectGroup>
        </SelectContent>
      </Select>
      <FormMessage />
    </FormItem>
  </FormField>

  <FormField v-slot="{ componentField }" name="issueDescription">
    <FormItem>
      <FormLabel>Details</FormLabel>
      <FormControl>
        <Textarea
          type="text"
          placeholder="Provide the details of the issues you have with your order."
          v-bind="componentField"
        />
      </FormControl>
      <FormMessage />
    </FormItem>
  </FormField>

  <FormField
    v-slot="{ componentField }"
    name="issueAttachments"
    :model-value="downloadUrlsString"
  >
    <FormItem>
      <FormLabel>Attachments</FormLabel>
      <FormDescription>
        Attach any files that are relevant to the issue you are having.
      </FormDescription>
      <FormControl>
        <Button
          class="block"
          type="button"
          @click="open({ accept: '*', multiple: true })"
        >
          Upload Files
        </Button>
        <Input
          type="hidden"
          v-bind="componentField"
          :value="downloadUrlsString"
        />
      </FormControl>
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
      <FormMessage />
    </FormItem>
  </FormField>
</template>
