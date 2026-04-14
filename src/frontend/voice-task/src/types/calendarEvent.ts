export interface CalendarEvent {
    eventId: string;
    title: string;
    startTime: string;
    endTime: string;
    location?: string;
    taskId?: string;
    externalAccountId?: string;
}