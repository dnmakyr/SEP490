import type { Issue } from '~/types/issues'

const mockIssues: Issue[] = [
  {
    issueId: '1a2b3c4d-1111-2222-3333-444455556666',
    issueName: 'Translation Error',
    createdAt: '2024-10-21T12:51:44.317Z',
    updatedAt: '2024-10-21T12:55:44.317Z',
    status: 'Open',
    clientName: 'ABC Corp',
    orderId: '1a2b3c4d-1111-2222-3333-444455556666',
    issueDescription: 'Translation error in the legal document.',
    assigneeName: 'John Doe',
    issueAttachments: [
      {
        issueId: '1a2b3c4d-1111-2222-3333-444455556666',
        attachmentUrl: 'https://example.com/attachments/doc1.pdf'
      }
    ]
  },
  {
    issueId: '1a2b3c4d-7777-8888-9999-111122223333',
    issueName: 'Incomplete File',
    createdAt: '2024-10-21T12:56:44.317Z',
    updatedAt: '2024-10-21T13:01:44.317Z',
    status: 'Closed',
    clientName: 'XYZ Ltd',
    orderId: '1a2b3c4d-7777-8888-9999-111122223333',
    issueDescription: 'Received an incomplete file for translation.',
    assigneeName: 'Jane Smith',
    issueAttachments: [
      {
        issueId: '1a2b3c4d-7777-8888-9999-111122223333',
        attachmentUrl: 'https://example.com/attachments/doc2.pdf'
      }
    ]
  },
  {
    issueId: '1a2b3c4d-4444-5555-6666-777788889999',
    issueName: 'Formatting Issue',
    createdAt: '2024-10-21T13:10:44.317Z',
    updatedAt: '2024-10-21T13:15:44.317Z',
    status: 'In Progress',
    clientName: 'Global Translations',
    orderId: '1a2b3c4d-4444-5555-6666-777788889999',
    issueDescription: 'Document formatting is incorrect.',
    assigneeName: 'Michael Johnson',
    issueAttachments: [
      {
        issueId: '1a2b3c4d-4444-5555-6666-777788889999',
        attachmentUrl: 'https://example.com/attachments/doc3.pdf'
      }
    ]
  },
  {
    issueId: '3fa85f64-1234-5678-9101-112131415161',
    issueName: 'Missing Translation',
    createdAt: '2024-10-21T13:20:44.317Z',
    updatedAt: '2024-10-21T13:25:44.317Z',
    status: 'Open',
    clientName: 'Language Services Inc.',
    orderId: '3fa85f64-1234-5678-9101-112131415161',
    issueDescription: 'Sections of the document are untranslated.',
    assigneeName: 'Emily Brown',
    issueAttachments: [
      {
        issueId: '3fa85f64-1234-5678-9101-112131415161',
        attachmentUrl: 'https://example.com/attachments/doc4.pdf'
      }
    ]
  },
  {
    issueId: '3fa85f64-9876-5432-1010-010101010101',
    issueName: 'Terminology Mismatch',
    createdAt: '2024-10-21T13:30:44.317Z',
    updatedAt: '2024-10-21T13:35:44.317Z',
    status: 'Resolved',
    clientName: 'Tech Solutions',
    orderId: '3fa85f64-9876-5432-1010-010101010101',
    issueDescription: 'Mismatch in the technical terms.',
    assigneeName: 'William Davis',
    issueAttachments: [
      {
        issueId: '3fa85f64-9876-5432-1010-010101010101',
        attachmentUrl: 'https://example.com/attachments/doc5.pdf'
      }
    ]
  },
  // 15 more entries following the same pattern...
  {
    issueId: '3fa85f64-2024-9876-1111-010203040506',
    issueName: 'Incorrect Language',
    createdAt: '2024-10-21T14:10:44.317Z',
    updatedAt: '2024-10-21T14:15:44.317Z',
    status: 'In Progress',
    clientName: 'Multilingual Experts',
    orderId: '3fa85f64-2024-9876-1111-010203040506',
    issueDescription: 'Document was translated into the wrong language.',
    assigneeName: 'Sophia Martin',
    issueAttachments: [
      {
        issueId: '3fa85f64-2024-9876-1111-010203040506',
        attachmentUrl: 'https://example.com/attachments/doc6.pdf'
      }
    ]
  }
]

export default mockIssues 
