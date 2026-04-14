import {format, isToday, parseISO} from "date-fns";
import {ru} from "date-fns/locale";

export function formatTime(dateStr: string) {
    return format(parseISO(dateStr), "HH:mm");
}

export function formatDateLabel(dateStr: string) {
    const date = parseISO(dateStr);
    if (isToday(date)) return "Сегодня";
    return format(date, "d MMMM, EEEE", { locale: ru });
}