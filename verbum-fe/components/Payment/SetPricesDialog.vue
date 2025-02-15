<script lang="ts" setup>
import { ref, watch, defineEmits } from 'vue'
import { Button } from '@/components/ui/button'
import type { Order } from '~/types/order'
import type { Language } from '~/types/language'
import { languages } from '~/constants/languages'

const props = defineProps<{
  order: Order
  open: boolean
  price: string
  supportedLanguage: Language[]
}>()
const emit = defineEmits(['close', 'confirm'])
const isOpen = ref(props.open)
const prices = ref(props.price)

watch(
  () => props.open,
  (newVal) => {
    isOpen.value = newVal
    prices.value = props.price

  }
)

const closeDialog = () => {
  emit('close')
}

const confirmPrice = () => {
  const priceValue = parseFloat(prices.value); // Convert to number
  if (isNaN(priceValue) || priceValue <= 0) {
    return;
  }
  emit('confirm', prices.value); // Emit event if valid
};

const totalSupportedLanguages = () => {
  let count = 0
  for (const item of props.supportedLanguage) {
    if (props.order.targetLanguageId) {
      for (const lang of props.order.targetLanguageId) {
        if (lang === item.languageId) {
          count++
        }
      }
    }
  }
  return count;
}

const total = totalSupportedLanguages();

const isSupported = (language: string) => {
  for (const item of props.supportedLanguage) {
    if (item.languageId === language) {
      return 'text-green-700 font-semibold'
    }
  }
}

const priceError = ref('');


watch(prices, (newVal) => {
  const priceValue = parseFloat(newVal);
  if (isNaN(priceValue) || priceValue <= 0 || priceValue >= 999999999) {
    priceError.value = 'Price must be a positive number.';
  } else {
    priceError.value = '';
  }
});

</script>

<template>
  <Dialog :open="isOpen" @click-outside="closeDialog">
    <DialogContent>
      <DialogTitle class="text-cyan-600 font-bold">Set Prices for {{ order.orderName }}</DialogTitle>
      <!-- <DialogDescription v-if="order.dueDate">Due date: {{order.dueDate}}</DialogDescription> -->
      <hr>
      <div class="flex w-full gap-3">
        <div class="flex-none w-1/3">
          <p class="font-semibold">Service:</p>
        </div>
        <div class="flex-auto">
          <ul>
            <li v-if="order.hasTranslateService">
              <span class="font-bold">TRN</span> - Translate
            </li>
            <li v-if="order.hasEditService">
              <span class="font-bold">EDIT</span> - Edit
            </li>
            <li v-if="order.hasEvaluateService">
              <span class="font-bold">EVL</span> - Evaluate
            </li>
          </ul>
        </div>
      </div>
      <hr>
      <div class="flex w-full gap-3">
        <div class="flex-none w-1/3">
          <p class="font-semibold">Source Language:</p>
          <p class="font-semibold">Target Language:</p>
        </div>
        <div class="flex-auto">
          
          <p
            :class="
              order.sourceLanguageId ? isSupported(order.sourceLanguageId) : ''
            "
          >
            {{ languages.find(
              (language) => language.languageId === order.sourceLanguageId
            )?.languageName }}
          </p>
          
          <ul v-for="item in order.targetLanguageId" :key="item">
            <li :class="isSupported(item)">{{ languages.find(
              (language) => language.languageId === item
            )?.languageName }}</li>
          </ul>
          <p class="text-xs italic text-gray-600">
            Order have <span class="text-green-700 font-bold">{{ total }}</span> supported languages for target languages.
          </p>
        </div>
      </div>
      <p class="text-sm italic font-semibold">
        Supported languages will be in
        <span class="text-green-700">green color</span>.
      </p>
      <hr>
      <p class="font-bold text-cyan-600">General Pricing Matrix</p>
      <div class="flex w-full">
        <div class="w-2/3">
          <p class="font-semibold">Price/hour:</p>
          <p class="font-semibold">Price/word/service:</p>
          <p class="font-semibold">Extra fee for unsupported languages:</p>
        </div>
        <div class="flex-none">
          <p class="text-cyan-600 font-semibold" >10 USD</p>
          <p class="text-cyan-600 font-semibold" >0.05 USD</p>
          <p class="text-cyan-600 font-semibold flex">
            <span><LucideArrowUp/></span>
            <span>20% - 30%</span>
          </p>
        </div>
      </div>
      <hr>
      <div>
        
        <label class="font-semibold">Price: </label>
        <input
          v-model="prices"
          class="border px-4 py-2 rounded-xl bg-gray-100 text-gray-900 dark:bg-gray-800 dark:text-gray-100 dark:border-gray-700"
          :class="priceError ? 'border-red-500' : 'border-gray-300'"
          type="number"
        > <span>USD</span>
        <p v-if="priceError" class="text-red-600 italic text-sm">Please enter a valid price</p>
      </div>
      <DialogFooter>
        <Button class="bg-slate-500 hover:bg-slate-600" @click="closeDialog">Cancel</Button>
        <Button @click="confirmPrice">Update</Button>
      </DialogFooter>
    </DialogContent>
  </Dialog>
</template>
