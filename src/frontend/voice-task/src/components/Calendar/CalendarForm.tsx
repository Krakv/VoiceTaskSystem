import { useState, useEffect } from "react";
import { calendarEventApi, type CalendarEventCreateDto } from "@/api/calendarEvent.api";
import { externalCalendarAccountApi } from "@/api/externalCalendarAccount.api";
import type { CalendarEvent } from "@/types/calendarEvent";
import type { ExternalCalendarAccount } from "@/types/externalCalendarAccount";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { DatePickerTime } from "@/components/DatePickerTime";
import { useNavigate } from "react-router-dom";
import { taskApi, GetTasksQuery } from "@/api/task.api";
import type { Task } from "@/types/task";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList } from "@/components/ui/combobox";
import { debounce } from "lodash";
import { useCallback } from "react";

interface CalendarEventFormProps {
    event?: CalendarEvent;
    onSuccess?: () => void;
}

export const CalendarEventForm = ({ event, onSuccess }: CalendarEventFormProps) => {
    const [title, setTitle] = useState(event?.title || "");
    const [startDate, setStartDate] = useState(event?.startTime || "");
    const [endDate, setEndDate] = useState(event?.endTime || "");
    const [location, setLocation] = useState(event?.location || "");

    const [taskId, setTaskId] = useState<string | undefined>(event?.taskId);
    const [taskName, setTaskName] = useState("");
    const [tasksList, setTasksList] = useState<Task[]>([]);

    const [externalAccountId, setExternalAccountId] = useState<string | undefined>(event?.externalAccountId);
    const [externalAccounts, setExternalAccounts] = useState<ExternalCalendarAccount[]>([]);

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        externalCalendarAccountApi.getExternalCalendarAccounts()
            .then(res => setExternalAccounts(res.data.data || []))
            .catch(() => setExternalAccounts([]));
    }, []);

    // Если редактируем и есть taskId — подгружаем имя задачи
    useEffect(() => {
        if (event?.taskId) {
            taskApi.getTasks(new GetTasksQuery({ query: "" }))
                .then(res => {
                    const found = (res.data.data.tasks || []).find((t: Task) => t.taskId === event.taskId);
                    if (found) setTaskName(found.title);
                })
                .catch(() => {});
        }
    }, [event?.taskId]);

    const debouncedFetchTasks = useCallback(
        debounce(async (query: string) => {
            if (!query) return setTasksList([]);
            try {
                const res = await taskApi.getTasks(new GetTasksQuery({ query }));
                setTasksList(res.data.data.tasks || []);
            } catch {
                setTasksList([]);
            }
        }, 300),
        []
    );

    useEffect(() => { debouncedFetchTasks(taskName); }, [taskName]);

    const handleSubmit = async () => {
        if (!title.trim()) {
            setError("Введите название события");
            return;
        }
        if (!startDate) {
            setError("Укажите время начала");
            return;
        }
        if (!endDate) {
            setError("Укажите время окончания");
            return;
        }

        setError("");
        setLoading(true);

        const data: CalendarEventCreateDto = {
            title,
            startTime: startDate,
            endTime: endDate,
            location: location || undefined,
            taskId: taskId || undefined,
            externalAccountId: externalAccountId || undefined,
        };

        try {
            if (event) {
                await calendarEventApi.updateCalendarEvent(event.eventId, data);
            } else {
                await calendarEventApi.createCalendarEvent(data);
            }
            onSuccess?.();
            navigate(-1);
        } catch {
            setError("Ошибка сохранения события");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto p-2 rounded-2xl space-y-6 pb-16">
            <h2 className="text-lg font-bold">
                {event ? "Редактирование события" : "Создание события"}
            </h2>

            {error && <div className="text-sm text-red-500">{error}</div>}

            <div className="space-y-2">
                <Label>Название</Label>
                <Input
                    placeholder="Например: Встреча с командой"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />
            </div>

            <div className="space-y-2">
                <Label>Начало</Label>
                <DatePickerTime value={startDate} onChange={setStartDate} />
            </div>

            <div className="space-y-2">
                <Label>Конец</Label>
                <DatePickerTime value={endDate} onChange={setEndDate} />
            </div>

            <div className="space-y-2">
                <Label>Место</Label>
                <Input
                    placeholder="Например: Офис, переговорная 2"
                    value={location}
                    onChange={(e) => setLocation(e.target.value)}
                />
            </div>

            <div className="space-y-2">
                <Label>Связанная задача</Label>
                <Combobox
                    value={tasksList.find(t => t.taskId === taskId) || null}
                    items={tasksList}
                    onValueChange={(task: Task | null) => {
                        setTaskId(task?.taskId);
                        setTaskName(task?.title || "");
                    }}
                >
                    <ComboboxInput
                        value={taskName}
                        placeholder="Выберите задачу"
                        onInput={(e) => setTaskName((e.target as HTMLInputElement).value)}
                    />
                    <ComboboxContent>
                        <ComboboxEmpty>Задача не найдена</ComboboxEmpty>
                        <ComboboxList>
                            {(item: Task) => (
                                <ComboboxItem key={item.taskId} value={item}>{item.title}</ComboboxItem>
                            )}
                        </ComboboxList>
                    </ComboboxContent>
                </Combobox>
            </div>

            {externalAccounts.length > 0 && (
                <div className="space-y-2">
                    <Label>Внешний календарь</Label>
                    <div className="flex flex-col gap-2">
                        <div
                            key="none"
                            onClick={() => setExternalAccountId(undefined)}
                            className={`cursor-pointer px-3 py-2 text-sm rounded-lg border transition-colors ${
                                !externalAccountId
                                    ? "bg-accent text-accent-foreground border-transparent"
                                    : "hover:bg-accent/50 border-border"
                            }`}
                        >
                            Без синхронизации
                        </div>
                        {externalAccounts.map((acc) => (
                            <div
                                key={acc.externalCalendarAccountId}
                                onClick={() => setExternalAccountId(acc.externalCalendarAccountId)}
                                className={`cursor-pointer px-3 py-2 text-sm rounded-lg border transition-colors ${
                                    externalAccountId === acc.externalCalendarAccountId
                                        ? "bg-accent text-accent-foreground border-transparent"
                                        : "hover:bg-accent/50 border-border"
                                }`}
                            >
                                {acc.baseUrl}
                            </div>
                        ))}
                    </div>
                </div>
            )}

            <Button onClick={handleSubmit} disabled={loading} className="w-full mt-2">
                {loading ? "Сохраняем..." : event ? "Сохранить" : "Создать событие"}
            </Button>
        </div>
    );
};