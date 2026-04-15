import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { calendarEventApi } from "@/api/calendarEvent.api";
import { CalendarEventForm } from "@/components/Calendar/CalendarForm";
import type { CalendarEvent } from "@/types/calendarEvent";

export const CalendarEditPage = () => {
    const { eventId } = useParams<{ eventId: string }>();

    const [event, setEvent] = useState<CalendarEvent | undefined>(undefined);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!eventId) return;

        const fetchEvent = async () => {
            try {
                const res = await calendarEventApi.getCalendarEventById(eventId);
                setEvent(res.data.data);
            } catch (e) {
                console.error(e);
            } finally {
                setLoading(false);
            }
        };

        fetchEvent();
    }, [eventId]);

    if (loading) {
        return (
            <div className="p-4 text-sm text-muted-foreground">
                Загрузка события...
            </div>
        );
    }

    if (!event) {
        return (
            <div className="p-4 text-sm text-red-500">
                Событие не найдено
            </div>
        );
    }

    return <CalendarEventForm event={event} />;
};