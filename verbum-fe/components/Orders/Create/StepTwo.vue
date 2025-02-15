<script setup lang="ts">
import { Checkbox } from '@/components/ui/checkbox'
import {
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage
} from '@/components/ui/form'
</script>

<template>
  <FormField
    v-slot="{ value, handleChange }"
    type="checkbox"
    name="hasTranslateService"
  >
    <FormItem
      class="flex flex-row items-start gap-x-3 space-y-0 rounded-md border p-4"
    >
      <FormControl>
        <Checkbox :checked="value" @update:checked="handleChange" />
      </FormControl>
      <div class="space-y-1 leading-none">
        <div class="text-primary font-bold">
          Translate
        </div>
        <FormLabel
          >Translate the document(s) from one to another language.</FormLabel
        >
        <FormDescription>
          This service is suitable for low-visibility, low-importance, and
          internal content.
        </FormDescription>
        <FormMessage />
      </div>
    </FormItem>
  </FormField>

  <FormField
    v-slot="{ value, handleChange }"
    type="checkbox"
    name="hasEditService"
  >
    <FormItem
      class="flex flex-row items-start gap-x-3 space-y-0 rounded-md border p-4"
    >
      <FormControl>
        <Checkbox :checked="value" @update:checked="handleChange" />
      </FormControl>
      <div class="space-y-1 leading-none">
        <div class="text-primary font-bold">
          Edit
        </div>
        <FormLabel
          >The existing translations will be reviewed and corrected using the
          source text</FormLabel
        >
        <FormDescription>
          Expect to eliminate the following error types: accuracy, completeness,
          style, spelling, and grammar.
        </FormDescription>
        <FormMessage />
      </div>
    </FormItem>
  </FormField>

  <FormField
    v-slot="{ value, handleChange }"
    type="checkbox"
    name="hasEvaluateService"
  >
    <FormItem
      class="flex flex-row items-start gap-x-3 space-y-0 rounded-md border p-4"
    >
      <FormControl>
        <Checkbox :checked="value" @update:checked="handleChange" />
      </FormControl>
      <div class="space-y-1 leading-none">
        <div class="text-primary font-bold">
          Evaluate
        </div>
        <FormLabel
          >The existing translations will be reviewed and corrected using the
          translation text only.</FormLabel
        >
        <FormDescription>
          Expect to eliminate the following error types: style, spelling, and
          grammar
        </FormDescription>
        <FormMessage />
      </div>
    </FormItem>
  </FormField>

  <FormField name="serviceValidate">
    <FormMessage />
  </FormField>
</template>
