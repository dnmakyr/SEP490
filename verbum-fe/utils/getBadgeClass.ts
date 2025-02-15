export const getIssueBadgeClass = (status: string | undefined) => {
  switch (status) {
    case 'OPEN':
      return 'bg-red-100 text-red-500 border-2 border-red-500'
    case 'IN_PROGRESS':
      return 'bg-yellow-100 text-yellow-500 border-2 border-yellow-500'
    case 'CANCEL':
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
    case 'SUBMITTED':
      return 'bg-blue-100 text-blue-500 border-2 border-blue-500'
    case 'RESOLVED':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
    default:
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
  }
}

export const getJobBadgeClass = (status: string | undefined) => {
  switch (status) {
    case 'NEW':
      return 'bg-red-100 text-red-500 border-2 border-red-500'
    case 'IN_PROGRESS':
      return 'bg-yellow-100 text-yellow-500 border-2 border-yellow-500'
    case 'SUBMITTED':
      return 'bg-blue-100 text-blue-500 border-2 border-blue-500'
    case 'APPROVED':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
    default:
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
  }
}

export const getReceiptBadgeClass = (status: string | undefined) => {
  switch (status) {
    case 'Deposit':
      return 'bg-blue-100 text-blue-500 border-2 border-blue-500'
    case 'Payment':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
    default:
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
  }
}
export const getWorkBadgeClass = (status: string | undefined) => {
  switch (status) {
    case 'NEW':
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
    case 'IN_PROGRESS':
      return 'bg-yellow-100 text-yellow-500 border-2 border-yellow-500'
    case 'SUBMITTED':
      return 'bg-blue-100 text-blue-500 border-2 border-blue-500'
    case 'APPROVED':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
      case 'COMPLETED':
      return 'bg-purple-100 text-purple-500 border-2 border-purple-500'
    case 'DELIVERED':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
    default:
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
  }
}

export const getOrderBadgeClass = (status: string | undefined) => {
  switch (status) {
    case 'NEW':
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
    case 'ACCEPTED':
      return 'bg-emerald-100 text-emerald-500 border-2 border-emerald-500'
    case 'REJECTED':
      return 'bg-red-100 text-red-500 border-2 border-red-500'
    case 'IN_PROGRESS':
      return 'bg-yellow-100 text-yellow-500 border-2 border-yellow-500'
    case 'COMPLETED':
      return 'bg-purple-100 text-purple-500 border-2 border-purple-500'
    case 'DELIVERED':
      return 'bg-green-100 text-green-500 border-2 border-green-500'
    default:
      return 'bg-gray-100 text-gray-500 border-2 border-gray-500'
  }
}
