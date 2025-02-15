 <script setup lang="ts">
import type { Order } from '~/types/order'
import ConfirmDialog from '~/components/Issues/ConfirmDialog.vue'
import SetPricesDialog from '~/components/Payment/SetPricesDialog.vue'
import { ORDER_COMPLETED, ORDER_IN_PROGRESS } from '~/constants/orderSatus'
import { useToast } from '~/components/ui/toast'
import { format } from 'date-fns'
import { formatToVietnamTimezone } from '#imports'
import { Calendar as CalendarIcon } from 'lucide-vue-next'
import {getLocalTimeZone, today, DateFormatter} from '@internationalized/date'
import { cn } from '@/lib/utils'
const { getRatingByOrderId, filteredRating } = useRating()

const { toast } = useToast()

const { supportedLanguages, getSupportedLanguages } = useLanguages()
const { order, isLoading, getOrder, setOrderPrice } =
  useOrders()
const route = useRoute()
const orderId = route.params.id
const { user } = useAuthStore()
const role = user?.role
const isEditing = ref(false)
const editedOrder = ref<Partial<Order> | null>(null)

const openSetPricesDialog = ref(false)
const openPaymentDialog = ref(false)
const openConfirmDialog = ref(false)
const openRatingDialog = ref(false)
const tempPrice = ref<string>('0')

onMounted(() => {
  getOrder(orderId)
  getSupportedLanguages()
  if (order.value) {
    useSeoMeta({ title: order.value.orderName })
  }
  getRating()
})

const getRating = () => {
  getRatingByOrderId(orderId as string)
  console.log({ filteredRating })
}

// Enter edit mode
const enableEdit = () => {
  isEditing.value = true
  if (order.value) {
    const {
      translationFileUrls,
      referenceFileUrls,
      jobDeliverables,
      createdDate,
      discountId,
      paymentStatus,
      orderStatus,
      ...rest
    } = order.value as Order
    editedOrder.value = {
      ...rest,
      targetLanguageId: [...(order.value.targetLanguageId || [])],
      sourceLanguageId: order.value.sourceLanguageId
    }
  }
}

// Cancel edit mode
const cancelEdit = () => {
  isEditing.value = false
  editedOrder.value = null
}

// Save edited order details
const saveEdit = async () => {
  try {
    if (editedOrder.value) {
      const payload = {
        ...editedOrder.value,
        dueDate: editedOrder.value.dueDate
          ? format(new Date(editedOrder.value.dueDate), "yyyy-MM-dd'T'HH:mm:ss")
          : null,
        targetLanguageIdList: editedOrder.value?.targetLanguageId,
        translateService: editedOrder.value?.hasTranslateService,
        editService: editedOrder.value?.hasEditService,
        evaluateService: editedOrder.value?.hasEvaluateService,
        discountId: editedOrder.value?.discountId
      }
      const { status } = await useAPI(`/order/update`, {
        method: 'PUT',
        body: JSON.stringify(payload),
        headers: {
          'Content-Type': 'application/json'
        }
      })
      if (status.value === 'success') {
        toast({
          title: 'Success',
          description: `Order updated successfully`
        })
        window.location.reload()
      }
    } else {
      throw new Error('No edited order found')
    }

    if (order.value) {
      Object.assign(order.value, editedOrder.value as Order)
    }
    isEditing.value = false
  } catch (error) {
    console.error('Failed to save order:', error)
  }
}

const changeSourceLanguage = (languageId: string) => {
  if (editedOrder.value) {
    editedOrder.value.sourceLanguageId = languageId
  }
}

const changeTargetLanguage = (languageIds: string[]) => {
  if (editedOrder.value) {
    editedOrder.value.targetLanguageId = languageIds
  }
}

interface Language {
  languageId: string
  languageName: string
  support: boolean
}
const languageList = ref<Language[]>([])

const orderRepo = repo(useNuxtApp().$api)

onMounted(async () => {
  try {
    const data = await orderRepo.getLanguages()
    languageList.value = data
  } catch (error) {
    console.error('Failed to fetch language list:', error)
  }
})
const handlePay = () => {
  openPaymentDialog.value = true
}

const handleSetPrices = () => {
  openSetPricesDialog.value = true
  refreshOrder()
  tempPrice.value = order.value?.orderPrice || '0'
}

const handlePaymentClose = () => {
  openPaymentDialog.value = false
  if (order.value?.orderStatus === 'DELIVERED') {
    openRatingDialog.value = true
  }
}

const handleRatingOpen = () => {
  openRatingDialog.value = true
}

const handleRatingClose = () => {
  openRatingDialog.value = false
  refreshOrder()
}

const handleRatingSubmit = async () => {
  openRatingDialog.value = false
  await refreshOrder()
}

const refreshOrder = async () => {
  if (orderId) {
    await getOrder(orderId)
  }
}

const confirmSetPrices = async () => {
  try {
    if (tempPrice.value !== null && order.value?.orderId) {
      // Make an API call to update the price
      console.log(tempPrice.value)
      await setOrderPrice(order.value?.orderId, tempPrice.value)
      // Update the local order price after successful API call
      order.value!.orderPrice = tempPrice.value
      openConfirmDialog.value = false // Close the confirmation dialog
      openSetPricesDialog.value = false // Close the Set Prices dialog
    }
  } catch (error) {
    console.error('Failed to set price:', error) // Log error if API call fails
  }
}

const orderTitle = computed(() => order.value?.orderName || 'Order Details')

const df = new DateFormatter('vi-VN', {
  dateStyle: 'short'
})

useSeoMeta({
  title: orderTitle
})
provide('role', role)
</script>

<template>
  <LoadingSpinner v-if="isLoading" />
  <div v-else>
    <div v-if="order" class="md:grid-cols-2 gap-5 pb-5">
      <OrdersStepper :order-status="order.orderStatus" />
      <!-- Order Information and File URLs Section -->
      <div class="mt-3 flex gap-3">
        <div class="flex-1 space-y-4">
          <div class="p-4 space-y-2 orderDetails border rounded-md">
            <div class="flex justify-between">
              <p class="text-[2rem] font-bold text-primary underline">
                {{ order?.orderName }}
              </p>
              <div class="flex gap-3 items-center">
                <h1 v-if="order.orderPrice" class="font-semibold text-xl">
                  Order Price:
                  <span class="font-bold hyper-link"
                    >{{ order?.orderPrice }} USD</span
                  ><br>
                  <span 
                  v-if="
                  role === 'CLIENT' &&
                  order.orderStatus === 'IN_PROGRESS' || order.orderStatus === 'COMPLETED'  &&
                  order.orderPrice" 
                  style="font-size: small; color: red;"><i>You have deposited 50% for your order.</i></span>
                </h1>
                <div
                  v-if="
                    order.orderStatus === 'ACCEPTED' &&
                    role === 'CLIENT' &&
                    order.orderPrice
                  "
                >
                  <Separator orientation="vertical" />
                  <Button @click="handlePay()">Deposit</Button>
                </div>
                <div
                  v-if="
                    order.orderStatus === 'COMPLETED' &&
                    role === 'CLIENT' &&
                    order.orderPrice
                  "
                >
                  <Separator orientation="vertical" />
                  <Button @click="handlePay()">Paying Remaining</Button>
                </div>
                <div
                  v-if="order.orderStatus === 'ACCEPTED' && role === 'DIRECTOR'"
                >
                  <Separator orientation="vertical" />
                  <Button @click="handleSetPrices">Set prices</Button>
                </div>
              </div>
            </div>

            <!-- Order Details -->
            <div class="flex flex-col gap-2 w-1/2">
              <h1 v-if="order.orderNote" class="font-semibold">
                Client Note:
                <span class="font-normal">{{ order?.orderNote }}</span>
              </h1>
              <h1 v-if="order.rejectReason" class="font-semibold text-red-600">
                Reject Reason:
                <span class="font-normal text-black">{{ order?.rejectReason}}</span>
              </h1>
              <h1 class="font-semibold">
                Status:
                <Badge :class="getOrderBadgeClass(order?.orderStatus)">{{
                  order?.orderStatus
                }}</Badge>
              </h1>

              <h1 v-if="order.createdDate" class="font-semibold">
                Created At:
                <span class="font-normal">{{
                  formatToVietnamTimezone(order?.createdDate || '')
                }}</span>
              </h1>
              <!-- Due Date -->
              <div class="flex items-center space-x-2">
                <h1 v-if="order.createdDate && !isEditing" class="font-semibold">
                  Due Date:
                  <span class="font-normal">{{
                    formatToVietnamTimezone(order?.dueDate || '')
                  }}</span>
                </h1>
                <template v-if="isEditing && editedOrder">
                  <h1 class="font-semibold">
                    Due Date:
                    <Popover>
                      <PopoverTrigger>
                        <Button
                          variant="outline"
                          :class="cn(
                            'w-[10rem] justify-start text-left font-normal',
                          )"
                        >
                          <CalendarIcon class="mr-2 h-4 w-4" />
                          {{ df.format(new Date(editedOrder.dueDate as string)) || "Pick a date" }}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent class="w-auto p-0">
                        <Calendar v-model="editedOrder.dueDate" initial-focus :max-value="today(getLocalTimeZone())"/>
                      </PopoverContent>
                    </Popover>
                  </h1>
                </template>
              </div>
              <h1 v-if="order.completedDate" class="font-semibold">
                Completed At:
                <span class="font-normal">{{
                  formatToVietnamTimezone(order.completedDate || '')
                }}</span>
              </h1>

              <h1 v-if="order.discountId" class="font-semibold">
                Discount:
                <span class="font-normal">{{ order.discountName }} - {{ order.discountAmount}}%</span>
              </h1>

              <!-- Services Section -->
              <div class="flex items-center space-x-3">
                <h1 class="font-semibold">Service:</h1>
                <template v-if="!isEditing">
                  <span
                    v-for="service in ['Translate', 'Edit', 'Evaluate']"
                    v-show="
                      (order?.hasTranslateService &&
                        service === 'Translate') ||
                      (order?.hasEditService && service === 'Edit') ||
                      (order?.hasEvaluateService && service === 'Evaluate')
                    "
                    :key="service"
                    class="font-bold text-primary"
                    >{{ service }}</span
                  >
                </template>
                <template v-else-if="editedOrder">
                  <span
                    v-for="service in ['Translate', 'Edit', 'Evaluate']"
                    :key="service"
                    class="font-bold text-primary"
                  >
                    <Checkbox
                      :id="`has${service}Service`"
                      v-model:checked="editedOrder[`has${service}Service`]"
                    />
                    {{ service }}
                  </span>
                </template>
              </div>

              <!-- Language Selection -->
              <div class="flex items-center space-x-1">
                <template v-if="!isEditing">
                  <Badge variant="default">{{
                    languageList.find(
                      (language) =>
                        language.languageId === order?.sourceLanguageId
                    )?.languageName
                  }}</Badge>
                  <LucideArrowBigRight />
                  <div class="flex gap-1">
                    <Badge
                      v-for="lang in order?.targetLanguageId || []"
                      :key="lang"
                      variant="secondary"
                    >
                      {{
                        languageList.find(
                          (language) => lang === language.languageId
                        )?.languageName || lang
                      }}
                    </Badge>
                  </div>
                </template>
                <template v-else>
                  <OrdersDetailsLanguageSelector
                    :language-list="languageList"
                    :selected-languages="editedOrder?.sourceLanguageId || ''"
                    :original-languages="order?.sourceLanguageId || ''"
                    :is-source-language="true"
                    @update:selected-languages="changeSourceLanguage"
                  />
                  <LucideArrowBigRight />
                  <OrdersDetailsLanguageSelector
                    :language-list="languageList"
                    :selected-languages="editedOrder?.targetLanguageId || []"
                    :original-languages="order?.targetLanguageId || []"
                    :is-source-language="false"
                    @update:selected-languages="changeTargetLanguage"
                  />
                </template>
              </div>
            </div>
          </div>

          <!-- Action Buttons -->
          <div class="flex space-x-2">
            <template v-if="isEditing && role === 'CLIENT'">
              <Button @click="saveEdit">Save</Button>
              <Button variant="outline" @click="cancelEdit">Cancel</Button>
            </template>
            <Button
              v-else-if="
                role === 'CLIENT' &&
                (order.orderStatus === 'NEW' ||
                  order.orderStatus === 'REJECTED')
              "
              @click="enableEdit"
              >Edit Order</Button
            >
            <OrdersDetailsCancelDialog
              v-if="
                role === 'CLIENT' &&
                order.orderStatus !== 'COMPLETED' &&
                order.orderStatus !== 'CANCELLED' &&
                order.orderStatus !== 'DELIVERED' &&
                order.orderStatus !== 'IN_PROGRESS'
              "
              :order-id="order.orderId"
            >
              <Button variant="destructive">Cancel Order</Button>
            </OrdersDetailsCancelDialog>
            <template v-if="role === 'STAFF'">
              <OrdersDetailsAcceptDialog v-if="order.orderStatus === 'NEW'" :order-id="order.orderId">
                <Button>Accept Order</Button>
              </OrdersDetailsAcceptDialog>
              <OrdersDetailsDialog
                v-if="order.orderStatus === 'NEW'"
                :order-id="order.orderId"
              />
            </template>
            <Button
              v-if="
                role === 'CLIENT' &&
                order.orderStatus === 'DELIVERED' &&
                !filteredRating
              "
              @click="handleRatingOpen"
              >Rating your order</Button
            >
          </div>

          <OrdersDetailsTabs :order="order" :is-edit="isEditing"/>

          <!-- Rating -->
          <div v-if="order.orderStatus === 'DELIVERED'">
            <RatingBox :order-id="orderId as string" />
          </div>
        </div>

        <!-- Smaller Issues List Section -->
        <OrdersIssues
          :job-deliverables="order.jobDeliverables"
          :order-id="orderId as string"
          :role="role"
          :user="user"
          :order-status="order.orderStatus"
        />
      </div>

      <!-- Set Prices Dialog -->
      <SetPricesDialog
        :order="order"
        :price="tempPrice"
        :open="openSetPricesDialog"
        :supported-language="supportedLanguages"
        @close="openSetPricesDialog = false"
        @confirm="
          (newPrice) => {
            openConfirmDialog = true
            tempPrice = newPrice
          }
        "
      />

      <!-- Confirm Dialog -->
      <ConfirmDialog
        :title="'Confirm Price Update'"
        :description="'Are you sure you want to update the price ?'"
        :open="openConfirmDialog"
        @close="openConfirmDialog = false"
        @confirm="confirmSetPrices"
      />

      <PaymentDialog
        :order="order"
        :status="order.orderStatus === 'ACCEPTED' ? 'IN_PROGRESS' : 'DELIVERED'"
        :open="openPaymentDialog"
        @close="handlePaymentClose"
      />

      <RatingDialog
        :open="openRatingDialog"
        :order-id="order.orderId"
        @close="handleRatingClose"
        @submit="handleRatingSubmit"
      />
    </div>
  </div>
</template>
