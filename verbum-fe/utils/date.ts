import { format, addHours, formatDistanceToNow } from 'date-fns'

const adjustToUserTimezone = (date: Date | string) => {
  return addHours(new Date(date), 7)
}

export const formatDistanceToNowUserTimezone = (date: Date | string) => {
  return formatDistanceToNow(adjustToUserTimezone(date), {
    addSuffix: true
  })
}

export function formatDate(
  date: Date | string,
  formatStr: string = 'dd MMMM yyyy'
) {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return format(dateObj, formatStr)
}

export const formatToVietnamTimezone = (date: Date | string) => {
  // const adjustedDate = addHours(new Date(date), 7);
  const adjustedDate = new Date(date);
  return format(adjustedDate, 'dd/MM/yyyy');
}