<script setup lang="ts">
import { cn } from '@/lib/utils'
import {
  DateFormatter,
  type DateValue,
  getLocalTimeZone,
  CalendarDate,
} from '@internationalized/date'
import { Calendar as CalendarIcon } from 'lucide-vue-next'

const df = new DateFormatter('en-US', {
  dateStyle: 'long',
})

const props = defineProps<{
  modelValue: Date | null | undefined
}>()

const emit = defineEmits<{
  'update:modelValue': [value: Date | null]
}>()

const value = computed({
  get: () => props.modelValue ? new CalendarDate(
    props.modelValue.getFullYear(),
    props.modelValue.getMonth() + 1,
    props.modelValue.getDate()
  ) : null,
  set: (newValue: DateValue | null) => {
    emit('update:modelValue', newValue instanceof CalendarDate ? newValue.toDate(getLocalTimeZone()) : null)
  }
})

const dateValue = computed(() => value.value ? value.value.toString() : '')
</script>

<template>
  <Popover>
    <PopoverTrigger as-child>
      <Button
        variant="outline"
        :class="cn(
          'w-[280px] justify-start text-left font-normal',
          !value && 'text-muted-foreground',
        )"
      >
        <CalendarIcon class="mr-2 h-4 w-4" />
        {{ dateValue || "Pick a date" }}
      </Button>
    </PopoverTrigger>
    <PopoverContent class="w-auto p-0">
      <Calendar v-model="value" initial-focus @click="console.log(dateValue)"/>
    </PopoverContent>
  </Popover>
</template>