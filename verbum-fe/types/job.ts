export interface Job {
    id: string;
    name: string;
    status: string;
    dueDate: string;
    workDueDate: string;
    createdAt: string;
    updatedAt: string;
    rejectReason: string;
    documentUrl: string;
    referenceUrls: string[];
    deliverableUrl: string;
    previousJobDeliverables: Record<string, string>;
    targetLanguageId: string;
    workId: string;
    assigneeNames: assigneeNames[];
    orderId: string;
}

export interface assigneeNames {
    id: string;
    name: string;
    email: string;
    roleCode: string;
    revelancies: string[];
}