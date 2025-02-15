<template>
  <div>
    <Tabs default-value="working" class="w-full">
      <TabsList class="grid w-full grid-cols-3">
        <TabsTrigger value="working">Working Files</TabsTrigger>
        <TabsTrigger value="reference">Reference Files</TabsTrigger>
        <TabsTrigger value="deliverable">Deliverable Files</TabsTrigger>
      </TabsList>
      <TabsContent value="working">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
            v-if="
              !Array.isArray(order.translationFileUrls) ||
              !order.translationFileUrls.length
            "
            class="p-2 text-center"
          >
            There are no working files, try refreshing the page
          </div>
          <Table v-else>
            <TableHeader>
              <TableRow>
                <TableHead class="flex gap-5 items-center">
                  <p class="text-primary">Files</p>
                  <Button
                    v-if="isEdit"
                    @click="
                      () => {
                        isTranslteFile = true
                        open({ accept: '*', multiple: true })
                      }
                    "
                    >Add Files</Button
                  >
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="file in order.translationFileUrls" :key="file">
                <TableCell class="font-semibold">{{
                  getFirebaseFileName(file)
                }}</TableCell>
                <TableCell>
                  <OrdersDetailsOptions
                    :id="order.orderId"
                    :url="file"
                    :is-deleted="false"
                    :is-delivered="false"
                    :is-new-or-rejected="isNewOrRejected"
                    :is-edit="props.isEdit"
                  />
                </TableCell>
              </TableRow>
            </TableBody>
            <Card
              v-if="
                isTranslteFile &&
                files?.length &&
                downloadUrlsString.length !== 0
              "
              :class="cn($attrs.class ?? '')"
            >
              <CardHeader>
                <CardDescription>Uploaded files</CardDescription>
              </CardHeader>
              <CardContent class="grid gap-3">
                <div
                  v-for="(file, index) in files"
                  :key="file.name"
                  class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
                >
                  <span
                    class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500"
                  />
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
                <div class="flex gap-3">
                  <Button variant="destructive" @click="handleCancelUpload"
                    >Cancel</Button
                  >
                  <Button
                    :disabled="downloadUrlsString.length === 0"
                    class="flex-1"
                    @click="handleUploadTranslteFile"
                    >Upload</Button
                  >
                </div>
              </CardContent>
            </Card>
          </Table>
        </div>
      </TabsContent>
      <TabsContent value="reference">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
            v-if="
              !Array.isArray(order.referenceFileUrls) ||
              !order.referenceFileUrls.length
            "
            class="p-2"
          >
            <Card
              v-if="isReferenceFile && files?.length"
              class="w-1/3"
              :class="cn($attrs.class ?? '')"
            >
              <CardHeader>
                <CardDescription>Uploaded files</CardDescription>
              </CardHeader>
              <CardContent class="grid gap-3">
                <div
                  v-for="(file, index) in files"
                  :key="file.name"
                  class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
                >
                  <span
                    class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500"
                  />
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
                <Button @click="handleCancelUpload">Cancel</Button>
                <Button @click="handleUploadReferenceFile">Upload</Button>
              </CardContent>
            </Card>
            <div v-else class="flex justify-center gap-2">
              <span>There are no reference files</span>
              <span
                class="hyper-link cursor-pointer"
                @click="
                  () => {
                    isReferenceFile = true
                    open({ accept: '*', multiple: true })
                  }
                "
                >Upload</span
              >
            </div>
          </div>
          <Table v-else>
            <TableHeader>
              <TableRow>
                <TableHead class="flex gap-5 items-center">
                  <p class="text-primary">Files</p>
                  <Button
                    v-if="isEdit"
                    @click="
                      () => {
                        isReferenceFile = true
                        open({ accept: '*', multiple: true })
                      }
                    "
                    >Add Files</Button
                  >
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="file in order.referenceFileUrls" :key="file">
                <TableCell class="font-semibold">{{
                  getFirebaseFileName(file)
                }}</TableCell>
                <TableCell>
                  <OrdersDetailsOptions
                    :id="order.orderId"
                    :url="file"
                    :is-deleted="false"
                    :is-delivered="false"
                    :is-new-or-rejected="isNewOrRejected"
                    :is-edit="props.isEdit"
                  />
                </TableCell>
              </TableRow>
            </TableBody>
            <Card
              v-if="
                isReferenceFile &&
                files?.length &&
                downloadUrlsString.length !== 0
              "
              class="w-1/2"
              :class="cn($attrs.class ?? '')"
            >
              <CardHeader>
                <CardDescription>Uploaded files</CardDescription>
              </CardHeader>
              <CardContent class="grid gap-3">
                <div
                  v-for="(file, index) in files"
                  :key="file.name"
                  class="mb-4 grid grid-cols-[25px_minmax(0,1fr)] items-start pb-4 last:mb-0 last:pb-0"
                >
                  <span
                    class="flex h-2 w-2 translate-y-1 rounded-full bg-sky-500"
                  />
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
                <div class="flex gap-3">
                  <Button variant="destructive" @click="handleCancelUpload"
                    >Cancel</Button
                  >
                  <Button
                    :disabled="downloadUrlsString.length === 0"
                    class="flex-1"
                    @click="handleUploadReferenceFile"
                    >Upload</Button
                  >
                </div>
              </CardContent>
            </Card>
          </Table>
        </div>
      </TabsContent>
      <TabsContent value="deliverable">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
            v-if="
              !Array.isArray(order.jobDeliverables) ||
              !order.jobDeliverables.length
            "
            class="p-2 text-center"
          >
            There are no deliverable files
          </div>
          <Table v-else>
            <TableHeader>
              <TableRow>
                <TableHead class="text-primary">Files</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow
                v-for="deliverable in getElementsWithHighestServiceOrder(
                  order.jobDeliverables
                )"
                :key="deliverable.deliverableFileUrl"
              >
                <TableCell class="font-semibold">{{
                  getFirebaseFileName(deliverable.deliverableFileUrl || '')
                }}</TableCell>
                <TableCell>
                  <OrdersDetailsOptions
                    :id="order.orderId"
                    :url="deliverable.deliverableFileUrl || ''"
                    :is-delivered="true"
                    :is-new-or-rejected="isNewOrRejected"
                    :is-edit="props.isEdit"
                  />
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>
      </TabsContent>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { getFirebaseFileName } from '@/utils/getFirebaseFileName'
import type { JobDeliverables } from '~/types/jobDeliverables'
import type { Order } from '~/types/order'
import { cn } from '@/lib/utils'

const { addOrderFile } = useOrders()

const props = defineProps({
  order: {
    type: Object as () => Order,
    default: () =>
      ({
        translationFileUrls: [],
        referenceFileUrls: [],
        deliverableFileUrls: [],
        deleteddFileUrls: [],
        orderId: ''
      }) as Order
  },
  isEdit: {
    type: Boolean,
    default: false
  }
})

const isTranslteFile = ref(false)
const isReferenceFile = ref(false)

const storage = useFirebaseStorage()
const { files, open, uploadProgress, downloadUrlsString, cancelUpload } =
  useFileUploader(storage)

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

const isNewOrRejected = computed(
  () =>
    props.order.orderStatus === 'NEW' || props.order.orderStatus === 'REJECTED'
)

const handleUploadTranslteFile = async () => {
  const urlsArray = downloadUrlsString.value.split(',').map((url) => ({
    orderId: props.order.orderId,
    referenceFileUrl: url,
    tag: 'TRANSLATION'
  }))
  await addOrderFile(urlsArray)
  refreshPage()
}

const handleUploadReferenceFile = async () => {
  const urlsArray = downloadUrlsString.value.split(',').map((url) => ({
    orderId: props.order.orderId,
    referenceFileUrl: url,
    tag: 'REFERENCES'
  }))
  await addOrderFile(urlsArray)
  refreshPage()
}

const handleCancelUpload = async () => {
  cancelUpload()
}
</script>
