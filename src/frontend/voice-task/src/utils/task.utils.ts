import {format, isToday, isTomorrow, isYesterday} from "date-fns";
import {ru} from "date-fns/locale";

export const statusMap = {
    new: {
        label: "Новая",
        className: "bg-gray-100 text-gray-600",
    },
    inProgress: {
        label: "В работе",
        className: "bg-blue-100 text-blue-700",
    },
    done: {
        label: "Готово",
        className: "bg-green-100 text-green-700",
    },
    canceled: {
        label: "Отменена",
        className: "bg-neutral-200 text-neutral-500",
    },
} as const;

export const priorityMap = {
    low: {
        label: "Низкий",
        className: "bg-green-50 text-green-700",
    },
    medium: {
        label: "Средний",
        className: "bg-yellow-50 text-yellow-700",
    },
    high: {
        label: "Высокий",
        className: "bg-red-50 text-red-700",
    },
} as const;

export const formatDate = (date?: string) => {
    if (!date) return null;

    return new Date(date).toLocaleDateString("ru-RU", {
        day: "numeric",
        month: "short",
    });
};

export const formatSmartDate = (dateStr?: string) => {
    if (!dateStr) return "";

    const date = new Date(dateStr);

    if (isToday(date)) {
        return `Сегодня, ${format(date, "HH:mm")}`;
    }

    if (isTomorrow(date)) {
        return `Завтра, ${format(date, "HH:mm")}`;
    }

    if (isYesterday(date)) {
        return "Вчера";
    }

    return format(date, "d MMM, HH:mm", { locale: ru });
};