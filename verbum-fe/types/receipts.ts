export interface Receipt {
    receiptId: string
    payDate: string
    depositeOrPayment: boolean
    amount: number
    orderId: string
    orderName: string
  }