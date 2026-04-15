import type {CalendarEvent} from "@/types/calendarEvent.ts";
import {format, isToday, parseISO} from "date-fns";
import {Badge} from "@/components/ui/badge.tsx";
import {Clock, MapPin} from "lucide-react";
import {ru} from "date-fns/locale";
import {formatTime} from "@/utils/calendar.utils.ts";

interface EventCardProps {
    event: CalendarEvent;
    showDate?: boolean;
    onClick?: () => void;
}

export const CalendarCard = ({ event, showDate, onClick }: EventCardProps) => {
    const start = parseISO(event.startTime);
    const todayEvent = isToday(start);

    return (
        <div className="flex items-start gap-3 px-4 py-3 rounded-2xl border border-slate-100 bg-white hover:bg-slate-50 transition-colors" onClick={onClick}>
            {/* Цветная полоска */}
            <div
                className={`mt-0.5 w-1 self-stretch rounded-full flex-shrink-0 ${
                    todayEvent ? "bg-slate-900" : "bg-slate-300"
                }`}
            />

            <div className="flex-1 min-w-0">
                <div className="flex items-start justify-between gap-2">
                    <span className="font-medium text-sm text-slate-900 leading-tight">
                        {event.title}
                    </span>
                    {todayEvent && (
                        <Badge
                            variant="secondary"
                            className="text-[10px] px-1.5 py-0 bg-slate-100 text-slate-600 flex-shrink-0"
                        >
                            Сегодня
                        </Badge>
                    )}
                </div>

                <div className="flex items-center gap-3 mt-1">
                    <span className="flex items-center gap-1 text-xs text-slate-500">
                        <Clock className="w-3 h-3" />
                        {formatTime(event.startTime)} — {formatTime(event.endTime)}
                    </span>
                    {showDate && !todayEvent && (
                        <span className="text-xs text-slate-400">
                            {format(start, "d MMM", { locale: ru })}
                        </span>
                    )}
                    {event.location && (
                        <span className="flex items-center gap-1 text-xs text-slate-400 truncate">
                            <MapPin className="w-3 h-3 flex-shrink-0" />
                            <span className="truncate">{event.location}</span>
                        </span>
                    )}
                </div>
            </div>
        </div>
    );
};
