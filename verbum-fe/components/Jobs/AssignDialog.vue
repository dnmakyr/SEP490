<script lang="ts" setup>
import type { Linguist } from '@/types/user'
import { toTypedSchema } from '@vee-validate/zod'
import { useForm } from 'vee-validate'
import * as z from 'zod'
import { cn } from '@/lib/utils'
import { CalendarIcon } from 'lucide-vue-next'
import { CalendarDate } from "@internationalized/date"
import { format } from 'date-fns'

const props = defineProps<{
  workDueDate: string
}>()
const assignList = inject<Ref<Linguist[]>>('assignList', ref([]))

const schema = toTypedSchema(z.object({
  assignee_id: z.array(z.string()).min(1, 'Please select at least one linguist'),
  dueDate: z.coerce.date().min(new Date(), 'Due date is required and must be today or greater').max(new Date(props.workDueDate), {
    message: 'Due date must be before the order\'s due date'
  }
  )
}))

const form = useForm({
  validationSchema: schema,
  initialValues: {
    assignee_id: [],
    dueDate: new Date()
  }
})

const selectedLinguists = ref<string[]>([])
const updateSelectedLinguists = (newSelectedLinguists: string[]) => {
  selectedLinguists.value = newSelectedLinguists
  form.setFieldValue('assignee_id', newSelectedLinguists)
}

const emit = defineEmits(['assign'])

const onSubmit = form.handleSubmit((values) => {
  const payload = {
    assigneesId: values.assignee_id,
    dueDate: format(values.dueDate, "yyyy-MM-dd'T'HH:mm:ss")
  }
  console.log(JSON.stringify(payload, null, 2))
  emit('assign', payload)
})
</script>
<template>
  <div>
    <Dialog>
      <DialogTrigger as-child><Button variant="outline">Assign</Button></DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Assign Job</DialogTitle>
          <DialogDescription class="text-black space-y-2">
            Choose linguists and due date
          </DialogDescription>
        </DialogHeader>
          <form 
            class="space-y-2" @submit="(e) => {
            e.preventDefault()
            onSubmit()
          }">
            <FormField v-slot="{ componentField, errorMessage }" name="assignee_id">
              <FormItem class="flex flex-col">
                <FormLabel>Linguist</FormLabel>
                <JobsLinguistSelector 
                  v-bind="componentField" v-model:selected-linguists="selectedLinguists"
                  :error="!!errorMessage" :linguists="assignList"
                  @update:selected-linguists="updateSelectedLinguists" />
                <FormMessage />
              </FormItem>
            </FormField>
            <FormField v-slot="{ componentField, value }" name="dueDate">
              <FormItem class="flex flex-col">
                <FormLabel>Due date</FormLabel>
                <Popover>
                  <PopoverTrigger as-child>
                    <Button 
                      variant="outline" :class="cn(
                      'w-[280px] justify-start text-left font-normal',
                      !value && 'text-muted-foreground',
                    )">
                      <CalendarIcon class="mr-2 h-4 w-4" />
                      {{ value ? format(new Date(value), 'dd/MM/yyyy') : "Pick a date" }}
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent class="w-auto p-0">
                    <Calendar 
                      v-bind="componentField" initial-focus :min-value="new CalendarDate(1900, 1, 1)"
                      :max-value="new Date(workDueDate)" />
                  </PopoverContent>
                </Popover>
                <FormMessage />
              </FormItem>
            </FormField>
            <DialogFooter>
              <Button type="submit" variant="outline">Confirm</Button>
            </DialogFooter>
          </form>
      </DialogContent>
    </Dialog>
  </div>
</template>