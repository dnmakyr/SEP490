<template>
  <div>
    <Tabs default-value="document" class="w-full">
      <TabsList class="grid w-full" :class="grids">
        <TabsTrigger value="document">Original Documents</TabsTrigger>
        <TabsTrigger value="references">References</TabsTrigger>
        <TabsTrigger value="deliverable">Deliverables</TabsTrigger>
        <TabsTrigger v-if="Object.keys(job.previousJobDeliverables).length > 0" value="relatedJob">Related Jobs
        </TabsTrigger>
      </TabsList>
      <TabsContent value="document">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
          v-if="
            !job.documentUrl
          " class="p-2 text-center">
            There are no working files
          </div>
          <div v-else class="p-2 flex justify-between items-center">
            <a class="hyper-link" :href="job.documentUrl">
              {{ getFirebaseFileName(job.documentUrl)}}
            </a>

            <Button variant="default" @click="download(job.documentUrl)">
              Download
            </Button>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="references">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
        v-if="
            !job.referenceUrls || job.referenceUrls.length === 0
          " class="p-2 text-center">
            There are no reference files
          </div>
          <div v-for="file in job.referenceUrls" v-else :key="file" class="p-2 flex justify-between items-center">
            <a class="hyper-link" :href="file">
              {{ getFirebaseFileName(file) }}
            </a>
            <Button variant="default" @click="download(file)">
              Download
            </Button>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="deliverable">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div 
          v-if="
            !job.deliverableUrl
          " class="p-2 text-center">
            There are no deliverables
          </div>
          <div v-else class="p-2 flex justify-between items-center">
            <a class="hyper-link" :href="job.deliverableUrl"> {{
            getFirebaseFileName(job.deliverableUrl) }}</a>
            <Button variant="default" @click="download(job.deliverableUrl)">
                Download
            </Button>
          </div>
        </div>
      </TabsContent>
      <TabsContent value="relatedJob">
        <div class="border rounded-md h-max-[18rem] overflow-auto">
          <div
          v-if="
            !(Object.keys(job.previousJobDeliverables).length > 0)
          " class="p-2 text-center">
            Previous service's deliverable has not been completed
          </div>
          <div v-else class="p-2">
            <div v-for="(label, file) in job.previousJobDeliverables" :key="file" class="flex justify-between items-center">
              <div class="space-x-6">
                <Badge>{{ label }}</Badge>
                <Badge>{{ targetLangId }}</Badge>
                <a :href="file" class="hyper-link">{{ getFirebaseFileName(file) }}</a>
              </div>
              <Button variant="default" @click="download(file)">
                Download
              </Button>
            </div>
          </div>
        </div>
      </TabsContent>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import type { Job } from '~/types/job';

const props = defineProps({
  job: {
    type: Object as PropType<Job> | undefined,
    default: () => ({
      documentUrl: '',
      deliverableUrl: '',
      previousJobDeliverables: {},
    })
  },
  targetLangId: {
    type: String as PropType<string>,
    default: ''
  }
})
const grids = computed(() => {
  return Object.keys(props.job.previousJobDeliverables).length > 0 ? 'grid-cols-4' : 'grid-cols-3'
})

const download = (url: string) => {
  window.open(url, '_blank')
}

</script>

<style></style>