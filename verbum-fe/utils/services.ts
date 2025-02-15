import { centerServices } from '~/constants/services'

export const getServiceName = (serviceCode: string) => {
  return (
    centerServices.find(
      (centerService) => serviceCode === centerService.serviceCode
    )?.serviceName || serviceCode
  )
}
