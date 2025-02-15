import type { JobDeliverables } from "./jobDeliverables"

export interface Order {
  orderId: string
  orderName?: string
  createdDate?: string
  dueDate?: string
  completedDate?: string
  sourceLanguageId?: string
  targetLanguageId?: string[]
  orderStatus?: string
  orderPrice?: string
  discountId?: string
  discountName?: string
  hasTranslateService?: boolean
  hasEditService?: boolean
  hasEvaluateService?: boolean
  orderNote?: string
  rejectReason?: string
  translationFileUrls?: string[]
  referenceFileUrls?: string[]
  jobDeliverables?: JobDeliverables[]
  deleteddFileUrls?: string[]
  paymentStatus?: string
  discountName?: string
  discountAmount?: number
}
