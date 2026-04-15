import {api} from "@/api/http.ts";
import type {NotificationServiceType} from "@/types/notification.ts";

export interface NotificationCreateDto {
    serviceId: NotificationServiceType;
    description: string;
    scheduledAt: string;
    taskId?: string | null;
}

export interface NotificationUpdateDto {
    description: string;
    scheduledAt: string;
}

export const notificationApi = {
    getNotifications: () => api.get("/notifications"),
    createNotification: (data: NotificationCreateDto) => api.post("/notifications", data),
    deleteNotification: (notificationId: string) => api.delete(`/notifications/${notificationId}`),
    getNotificationById: (notificationId: string) => api.get(`/notifications/${notificationId}`),
    updateNotification: (notificationId: string, data: NotificationUpdateDto) => api.put(`/notifications/${notificationId}`, data)
};