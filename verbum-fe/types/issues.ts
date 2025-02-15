export type IssueAttachments = {
  attachmentUrl: string
  tag:string
}

export type Issue = {
  issueId: string
  issueName: string
  createdAt: string
  updatedAt: string
  status: string
  clientName: string
  orderId: string
  orderName: string
  issueDescription: string
  assigneeName: string
  assigneeId: string
  issueAttachments: IssueAttachments[]
  cancelResponse: string
  rejectResponse: string
  documentUrl: string
}

export type IssueUpdatePayload = {
  issueId: string
  issueName: string
  issueDescription: string
  assigneeId: string
  issueAttachments: IssueAttachments[]
}

export type IssueReOpenPayload = {
    issueId: string
    issueName: string
    issueDescription: string
    issueAttachments: IssueAttachments[]
}
