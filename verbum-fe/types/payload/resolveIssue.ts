import type { IssueAttachments } from '~/types/issues'

export interface ResolveIssuePayload {
  issueId: string;
  issueName: string
  issueDescription: string
  assigneeId: string
  issueAttachments: IssueAttachments[]
}
