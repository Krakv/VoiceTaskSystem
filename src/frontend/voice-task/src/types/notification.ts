import type {ShortTaskInfo} from "@/types/task.ts";

export type NotificationStatus = "pending" | "processing" | "sent" | "failed" | "cancelled";

export type NotificationServiceType = "telegram" | "email";

export interface NotificationItem {
    notificationId: string;
    taskId?: string;
    task: ShortTaskInfo;
    serviceId: NotificationServiceType;
    description: string;
    scheduledAt: string;
    sentAt?: string;
    status: NotificationStatus;
}