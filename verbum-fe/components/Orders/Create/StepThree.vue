<script setup lang="ts">
import { useFileDialog } from '@vueuse/core'
import { ref, watch, computed } from 'vue'
import { cn } from '@/lib/utils'
import { ref as storageRef, getDownloadURL, uploadBytesResumable } from 'firebase/storage'

const storage = useFirebaseStorage()
const downloadUrls = ref<string[]>([])
const uploadProgress = ref<number[]>([]) // Track progress of each file

const {getDiscountById} = useDiscounts()

const downloadUrlsString = computed(() => downloadUrls.value.join(','))

const discountCode = ref('')
const appliedDiscount = ref<string | null>(null)

async function applyDiscount() {
  const response = await getDiscountById(discountCode.value)
  if (response) {
    appliedDiscount.value = response.discountId
  } else {
    appliedDiscount.value = null
  }
}

async function uploadFiles() {
  downloadUrls.value = []
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

</script>

<template>
  <FormField v-slot="{ componentField }" name="orderNote">
    <FormItem>
      <FormLabel>Note</FormLabel>
      <FormControl>
        <Textarea
          placeholder="Tell us further details, notes, references, instructions or any special requests"
          v-bind="componentField"
          rows="5"
        />
      </FormControl>
      <FormMessage />
    </FormItem>
  </FormField>

  <FormField
    v-slot="{ componentField }"
    name="referenceFileURLs"
    :model-value="downloadUrlsString"
  >
    <FormItem class="flex flex-col">
      <FormLabel>Reference Files</FormLabel>
      <FormControl>
        <Button type="button" @click="open({ accept: '*', multiple: true })">
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

  <FormField v-slot="{ componentField }" name="discountId" :model-value="appliedDiscount">
    <FormItem>
      <FormLabel>Discount code</FormLabel>
      <FormControl>
        <div class="flex gap-3">
          <Input
          v-model="discountCode"
          placeholder="Enter you discount code here"
          />
          <Input
          type="hidden"
          v-bind="componentField"
          :value="appliedDiscount ? appliedDiscount.valueOf() : null"
          />
          <Button type="button" @click.prevent="applyDiscount">Apply</Button>
        </div>
      </FormControl>
      <FormMessage />
    </FormItem>
  </FormField>
</template>
