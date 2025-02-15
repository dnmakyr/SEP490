<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { Calendar } from '@/components/ui/v-calendar'
import { Check, ChevronDown, Calendar as CalendarIcon } from 'lucide-vue-next'
import { cn } from '@/lib/utils'
import { Button } from '@/components/ui/button'
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList
} from '@/components/ui/command'
import {
  Popover,
  PopoverContent,
  PopoverTrigger
} from '@/components/ui/popover'
import { format } from 'date-fns'
import { useFileDialog } from '@vueuse/core'
import { ref as storageRef, getDownloadURL, uploadBytesResumable } from 'firebase/storage'
import { useAPI } from '@/composables/useCustomFetch'
import type { Language } from '~/types/language'

const openTarget = ref(false)
const selectedValues = ref<string[]>([])
const languages = ref<Language[]>([])

const selectedValuesString = computed(() => selectedValues.value.join(','))

const sourceLanguage = ref<string>('');
const openSourceLanguagePopover = ref(false);

const toggleSelection = (value: string) => {
  if (selectedValues.value.includes(value)) {
    selectedValues.value = selectedValues.value.filter((v) => v !== value)
  } else {
    selectedValues.value.push(value)
  }
}

const storage = useFirebaseStorage()
const downloadUrls = ref<string[]>([])
const uploadProgress = ref<number[]>([])

const downloadUrlsString = computed(() => downloadUrls.value.join(','))

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

onMounted(async () => {
  const { data, error } = await useAPI('/lang')
  if (!error.value) {
    languages.value = (data.value as Language[]) || []
  }
})
</script>

<template>
  <div>
    <FormField
      v-slot="{ componentField }"
      name="translationFileURL"
      :model-value="downloadUrlsString"
    >
      <FormItem class="flex flex-col">
        <FormLabel>Files</FormLabel>
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

    <FormField
      v-slot="{ componentField }"
      name="sourceLanguageId"
      :model-value="sourceLanguage"
    >
      <FormItem>
        <FormLabel>Source Language</FormLabel>
        <FormControl>
          <Input
            v-bind="componentField"
            :value="sourceLanguage"
            type="hidden"
          />
        </FormControl>
        <Popover v-model:open-target="openSourceLanguagePopover">
          <PopoverTrigger as-child>
            <Button
              variant="outline"
              role="combobox"
              :aria-expanded="openSourceLanguagePopover"
              class="w-full justify-between"
            >
              <span class="font-normal">
                {{
                  sourceLanguage
                    ? languages.find(
                        (language: Language) =>
                          language.languageId === sourceLanguage
                      )?.languageName
                    : 'Select source language...'
                }}
              </span>
              <ChevronDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent class="p-0">
            <Command>
              <CommandInput
                class="h-9"
                placeholder="Search source language..."
              />
              <CommandEmpty>No language found.</CommandEmpty>
              <CommandList>
                <CommandGroup>
                  <CommandItem
                    v-for="language in languages"
                    :key="language.languageId"
                    :value="language.languageId"
                    @select="sourceLanguage = language.languageId"
                  >
                    {{ language.languageName }}
                    <Check
                      :class="
                        cn(
                          'ml-auto h-4 w-4',
                          sourceLanguage === language.languageId
                            ? 'opacity-100'
                            : 'opacity-0'
                        )
                      "
                    />
                  </CommandItem>
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>
        <FormMessage />
      </FormItem>
    </FormField>


    <FormField
      v-slot="{ componentField }"
      name="targetLanguageIdList"
      :model-value="selectedValuesString"
    >
      <FormItem>
        <FormLabel>Target Languages</FormLabel>
        <FormControl>
          <Input
            v-bind="componentField"
            :value="selectedValuesString"
            type="hidden"
          />
        </FormControl>
        <Popover v-model:open-target="openTarget">
          <PopoverTrigger as-child>
            <Button
              variant="outline"
              role="combobox"
              :aria-expanded="open"
              class="w-full justify-between"
            >
              <span class="font-normal">
                {{
                  selectedValues.length
                    ? selectedValues
                        .map(
                          (val) =>
                            languages.find(
                              (language: Language) =>
                                language.languageId === val
                            )?.languageName
                        )
                        .join(', ')
                    : 'Select target language...'
                }}
              </span>
              <ChevronDown class="ml-2 h-4 w-4 shrink-0 opacity-50" />
            </Button>
          </PopoverTrigger>
          <PopoverContent class="p-0">
            <Command>
              <CommandInput
                class="h-9"
                placeholder="Search target language..."
              />
              <CommandEmpty>No language found.</CommandEmpty>
              <CommandList>
                <CommandGroup>
                  <CommandItem
                    v-for="language in languages"
                    :key="language.languageId"
                    :value="language.languageId"
                    @select="toggleSelection(language.languageId)"
                  >
                    {{ language.languageName }}
                    <Check
                      :class="
                        cn(
                          'ml-auto h-4 w-4',
                          selectedValues.includes(language.languageId)
                            ? 'opacity-100'
                            : 'opacity-0'
                        )
                      "
                    />
                  </CommandItem>
                </CommandGroup>
              </CommandList>
            </Command>
          </PopoverContent>
        </Popover>
        <FormMessage />
      </FormItem>
    </FormField>

    <FormField v-slot="{ componentField, value }" name="dueDate">
      <FormItem class="flex flex-col">
        <FormLabel>Due date</FormLabel>
        <Popover>
          <PopoverTrigger as-child>
            <FormControl>
              <Button
                variant="outline"
                :class="
                  cn(
                    'w-[240px] ps-3 text-start font-normal',
                    !value && 'text-muted-foreground'
                  )
                "
              >
                <span>{{ value ? format(value, 'PPP') : 'Pick a date' }}</span>
                <CalendarIcon class="ms-auto h-4 w-4 opacity-50" />
              </Button>
            </FormControl>
          </PopoverTrigger>
          <PopoverContent class="p-0">
            <Calendar v-bind="componentField" />
          </PopoverContent>
        </Popover>
        <FormMessage />
      </FormItem>
    </FormField>
  </div>
</template>
