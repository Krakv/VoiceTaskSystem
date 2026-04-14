import type {ShortTaskInfo} from "@/types/task.ts";

export type NotificationStatus = "pending" | "processing" | "sent" | "failed" | "cancelled";

export interface NotificationItem {
    notificationId: string;
    taskId?: string;
    task: ShortTaskInfo;
    serviceId: number;
    description: string;
    scheduledAt: string;
    sentAt?: string;
    status: NotificationStatus;
}