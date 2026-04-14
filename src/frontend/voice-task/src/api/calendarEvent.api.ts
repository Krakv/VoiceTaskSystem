import {api} from "@/api/http.ts";

export interface CalendarEventCreateDto {
    title: string;
    startTime: string;
    endTime: string;
    location?: string;
    taskId?: string;
    externalAccountId?: string;
}

export interface CalendarEventUpdateDto {
    title: string;
    startTime: string;
    endTime: string;
    location?: string;
    taskId?: string;
    externalAccountId?: string;
}

export const calendarEventApi = {
    getCalendarEvents: () => api.get("/calendar-events"),
    createCalendarEvent: (data: CalendarEventCreateDto) => api.post("/calendar-events", data),
    deleteCalendarEvent: (calendarEventId: string) => api.delete(`/calendar-events/${calendarEventId}`),
    getCalendarEventById: (calendarEventId: string) => api.get(`/calendar-events/${calendarEventId}`),
    updateCalendarEvent: (calendarEventId: string, data: CalendarEventUpdateDto) => api.put(`/calendar-events/${calendarEventId}`, data)
};