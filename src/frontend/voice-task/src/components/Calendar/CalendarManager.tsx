import { useEffect, useState } from "react";
import { calendarEventApi } from "@/api/calendarEvent.api";
import type { CalendarEvent } from "@/types/calendarEvent";
import { Calendar } from "@/components/ui/calendar";
import { Skeleton } from "@/components/ui/skeleton";
import {
    TooltipProvider,
} from "@/components/ui/tooltip";
import { format, parseISO, isToday, isFuture } from "date-fns";
import {CalendarDay, type Modifiers} from "react-day-picker";
import { ru } from "date-fns/locale";
import {Plus} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";
import {useNavigate} from "react-router-dom";
import {CalendarCard} from "@/components/Calendar/CalendarCard.tsx";
import {DayEventsTooltip} from "@/components/Calendar/DayEventsTooltip.tsx";
import {formatDateLabel} from "@/utils/calendar.utils.ts";
import {CalendarEventSheet} from "@/components/Calendar/CalendarSheet.tsx";

// Группировка событий по дате (YYYY-MM-DD)
function groupEventsByDate(events: CalendarEvent[]): Record<string, CalendarEvent[]> {
    return events.reduce((acc, event) => {
        const key = format(parseISO(event.startTime), "yyyy-MM-dd");
        if (!acc[key]) acc[key] = [];
        acc[key].push(event);
        return acc;
    }, {} as Record<string, CalendarEvent[]>);
}

export const CalendarManager = () => {
    const [events, setEvents] = useState<CalendarEvent[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedDate, setSelectedDate] = useState<Date | undefined>(new Date());
    const [selectedEvent, setSelectedEvent] = useState<CalendarEvent | null>(null);
    const [sheetOpen, setSheetOpen] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchEvents = async () => {
            setLoading(true);
            try {
                const res = await calendarEventApi.getCalendarEvents();
                setEvents(res.data.data ?? []);
            } catch (e) {
                console.error(e);
            } finally {
                setLoading(false);
            }
        };
        fetchEvents();
    }, []);

    const handleCreate = () => {
        navigate("/calendar/create");
    };

    const handleOpenEvent = (event: CalendarEvent) => {
        setSelectedEvent(event);
        setSheetOpen(true);
    };

    const grouped = groupEventsByDate(events);

    // Ближайшие события (сегодня и будущие, отсортированные)
    const upcomingEvents = events
        .filter((e) => {
            const d = parseISO(e.startTime);
            return isToday(d) || isFuture(d);
        })
        .sort((a, b) => parseISO(a.startTime).getTime() - parseISO(b.startTime).getTime())
        .slice(0, 10);

    // События выбранного дня
    const selectedDayKey = selectedDate
        ? format(selectedDate, "yyyy-MM-dd")
        : null;
    const selectedDayEvents = selectedDayKey ? grouped[selectedDayKey] ?? [] : [];

    return (
        <div className="flex flex-col gap-6 pb-24">
            {/* Большой календарь */}
            <div className="rounded-2xl border border-slate-200 bg-white shadow-sm overflow-hidden">
                {loading ? (
                    <Skeleton className="h-[340px] w-full rounded-2xl" />
                ) : (
                    <TooltipProvider>
                        <Calendar
                            mode="single"
                            selected={selectedDate}
                            onSelect={setSelectedDate}
                            locale={ru}
                            className="w-full p-4"
                            classNames={{
                                months: "w-full",
                                month: "w-full space-y-3",
                                caption: "flex justify-between items-center px-1 mb-1",
                                caption_label: "text-sm font-semibold text-slate-900 capitalize",
                                nav: "flex items-center gap-1",
                                nav_button:
                                    "h-7 w-7 rounded-lg border border-slate-200 bg-white hover:bg-slate-50 flex items-center justify-center transition-colors",
                                nav_button_previous: "",
                                nav_button_next: "",
                                table: "w-full border-collapse",
                                head_row: "flex w-full mb-1",
                                head_cell:
                                    "flex-1 text-center text-[11px] font-medium text-slate-400 uppercase tracking-wider py-1",
                                row: "flex w-full mt-0.5",
                                cell: "flex-1 text-center relative p-0",
                                day: "mx-auto w-9 h-9 rounded-xl text-sm font-medium text-slate-700 hover:bg-slate-100 transition-colors flex items-center justify-center",
                                day_selected: "bg-slate-950 text-white hover:bg-slate-800 rounded-xl",
                                day_today: "text-slate-900 font-bold",
                                day_outside: "text-slate-300",
                                day_disabled: "text-slate-300 cursor-not-allowed",
                            }}
                            components={{
                                DayButton: ({ day, modifiers, ...props }: { day: CalendarDay; modifiers: Modifiers } & React.ButtonHTMLAttributes<HTMLButtonElement>) => {
                                    const key = format(day.date, "yyyy-MM-dd");
                                    const dayEvents = grouped[key];
                                    const isSelected = modifiers.selected;
                                    const hasEvents = !!dayEvents?.length;

                                    const inner = (
                                        <button {...props}>
                                            {day.date.getDate()}
                                            {hasEvents && (
                                                <span className="absolute bottom-0.5 left-1/2 -translate-x-1/2 flex gap-0.5">
                                                    {dayEvents.slice(0, 3).map((_, i) => (
                                                        <span
                                                            key={i}
                                                            className={`w-1 h-1 rounded-full ${isSelected ? "bg-white/70" : "bg-slate-400"}`}
                                                        />
                                                    ))}
                                                </span>
                                            )}
                                        </button>
                                    );

                                    if (!hasEvents) return <button {...props} />;

                                    return <DayEventsTooltip events={dayEvents}>{inner}</DayEventsTooltip>;
                                }
                            }}
                        />
                    </TooltipProvider>
                )}
            </div>

            {/* События выбранного дня */}
            {selectedDate && (
                <div>
                    <h2 className="text-sm font-semibold text-slate-900 mb-2">
                        {formatDateLabel(selectedDate.toISOString())}
                    </h2>
                    {loading ? (
                        <Skeleton className="h-14 w-full rounded-xl" />
                    ) : selectedDayEvents.length === 0 ? (
                        <div className="text-sm text-slate-400 bg-slate-50 rounded-xl px-4 py-3">
                            Событий нет
                        </div>
                    ) : (
                        <div className="flex flex-col gap-2">
                            {selectedDayEvents.map((event) => (
                                <CalendarCard
                                    key={event.eventId}
                                    event={event}
                                    onClick={() => handleOpenEvent(event)}
                                />
                            ))}
                        </div>
                    )}
                </div>
            )}

            {/* Ближайшие события */}
            <div>
                <h2 className="text-sm font-semibold text-slate-900 mb-2">Ближайшие события</h2>
                {loading ? (
                    <div className="flex flex-col gap-2">
                        <Skeleton className="h-14 w-full rounded-xl" />
                        <Skeleton className="h-14 w-full rounded-xl" />
                        <Skeleton className="h-14 w-full rounded-xl" />
                    </div>
                ) : upcomingEvents.length === 0 ? (
                    <div className="text-sm text-slate-400 bg-slate-50 rounded-xl px-4 py-3">
                        Предстоящих событий нет
                    </div>
                ) : (
                    <div className="flex flex-col gap-2">
                        {upcomingEvents.map((event) => (
                            <CalendarCard
                                key={event.eventId}
                                event={event}
                                onClick={() => handleOpenEvent(event)}
                            />
                        ))}
                    </div>
                )}
            </div>

            <div className="fixed bottom-16 right-4 flex max-w-md justify-end">
                <Button
                    onClick={handleCreate}
                    className="relative rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
                >
                    <Plus className="w-10 h-10" />
                </Button>
            </div>

            <CalendarEventSheet
                event={selectedEvent}
                open={sheetOpen}
                onOpenChange={setSheetOpen}
                onEdit={(id) => navigate(`/calendar/${id}/edit`)}
                onDelete={async (id) => {
                    await calendarEventApi.deleteCalendarEvent(id);

                    setEvents((prev) => prev.filter((e) => e.eventId !== id));
                    setSheetOpen(false);
                }}
                onOpenTask={(taskId) => navigate(`/tasks/${taskId}`)}
            />
        </div>
    );
};


