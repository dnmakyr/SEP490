<script setup lang="ts">
import { languages } from '~/constants/languages'
import { getWorkBadgeClass } from '~/utils/getBadgeClass'
const { isLoading, work, getWorkById } = useWorks()
const { jobs, getJobsOfWork } = useJobs()
const { assignList, getAssignList } = useUsers()
const role = useAuthStore().user?.role as string | undefined
const route = useRoute()
const workId = route.params.id as string
onMounted(async () => {
  await getJobsOfWork(workId)
  await getWorkById(workId)
  if (role?.includes('MANAGER')) {
    await getAssignList()
  }
})

const currentService = computed(() => {
  switch (work.value?.serviceCode) {
    case 'TL':
      return 'Translate'
    case 'ED':
      return 'Edit'
    case 'EV':
      return 'Evaluate'
    default:
      return ''
  }
})

provide('assignList', assignList)
</script>

<template>
  <LoadingSpinner v-if="isLoading" />
  <div v-else class="h-full overflow-y-hidden">
    <div v-if="work" class="flex flex-col mb-4 h-full">
      <div class="flex space-x-4">
        <div class="space-y-2">
          <p class="text-[2rem] font-bold text-primary underline">
            {{ work?.workName }}
          </p>
          <Badge
            :class="work?.orderStatus === 'IN_PROGRESS' && work.isCompleted
                ? getWorkBadgeClass('COMPLETED')
                : getWorkBadgeClass(work?.orderStatus)"
          >
            {{
              work?.orderStatus === 'IN_PROGRESS' && work.isCompleted
                ? 'COMPLETED'
                : work?.orderStatus
            }}
          </Badge>
          <h1 class="font-semibold space-x-2 mt-4">
            Service(s): 
            <span 
              v-for="service in ['Translate', 'Edit', 'Evaluate']"
              v-show="
              work.translateService && service === 'Translate'
              || work.editService && service === 'Edit'
              || work.evaluateService && service === 'Evaluate'
              "
              :key="service"
              class="font-semibold text-primary"
              :class="currentService.includes(service) ? 'underline' : ''">
              {{ service }}
            </span>
          </h1>
          <h1 class="font-semibold">
            Due Date:
            <span class="font-bold hyper-link">{{
              formatToVietnamTimezone(work?.dueDate)
            }}</span>
          </h1>
          <div class="flex gap-2">
            <Badge variant="default">{{
              languages.find(
                (language) => language.languageId === work?.sourceLanguageId
              )?.languageName
            }}</Badge>
            <LucideArrowBigRight />
            <Badge
              v-for="lang in work?.targetLanguageId || []"
              :key="lang"
              variant="secondary"
            >
              {{ getLanguageName(lang) }}
            </Badge>
          </div>
        </div>
      </div>
      <WorksStatusColumn :data="jobs" />
    </div>
  </div>
</template>
