import type {CalendarEvent} from "@/types/calendarEvent.ts";
import {Tooltip, TooltipContent, TooltipProvider, TooltipTrigger} from "@/components/ui/tooltip.tsx";
import {Clock, MapPin} from "lucide-react";
import {formatTime} from "@/utils/calendar.utils.ts";

interface DayEventsTooltipProps {
    events: CalendarEvent[];
    children: React.ReactNode;
}

export const DayEventsTooltip = ({ events, children }: DayEventsTooltipProps) => (
    <TooltipProvider delayDuration={100}>
        <Tooltip>
            <TooltipTrigger asChild>{children}</TooltipTrigger>
            <TooltipContent
                side="top"
                className="w-64 p-0 overflow-hidden rounded-xl shadow-xl border-0"
            >
                <div className="bg-white">
                    <div className="px-3 py-2 bg-slate-950 text-white text-xs font-semibold tracking-wide uppercase">
                        {events.length} {events.length === 1 ? "событие" : "события"}
                    </div>
                    <div className="divide-y divide-slate-100">
                        {events.map((event) => (
                            <div key={event.eventId} className="px-3 py-2.5">
                                <div className="font-medium text-sm text-slate-900 leading-tight">
                                    {event.title}
                                </div>
                                <div className="flex items-center gap-1 mt-1 text-xs text-slate-500">
                                    <Clock className="w-3 h-3" />
                                    <span>
                                        {formatTime(event.startTime)} — {formatTime(event.endTime)}
                                    </span>
                                </div>
                                {event.location && (
                                    <div className="flex items-center gap-1 mt-0.5 text-xs text-slate-400">
                                        <MapPin className="w-3 h-3" />
                                        <span className="truncate">{event.location}</span>
                                    </div>
                                )}
                            </div>
                        ))}
                    </div>
                </div>
            </TooltipContent>
        </Tooltip>
    </TooltipProvider>
);