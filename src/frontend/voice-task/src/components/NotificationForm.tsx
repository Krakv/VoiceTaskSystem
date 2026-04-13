import { useState, useEffect, useCallback } from "react";
import { notificationApi } from "@/api/notification.api";
import type { NotificationItem } from "@/types/notification";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList } from "@/components/ui/combobox";
import { DatePickerTime } from "@/components/DatePickerTime";
import { taskApi, GetTasksQuery } from "@/api/task.api";
import { debounce } from "lodash";
import { useNavigate } from "react-router-dom";
import type { Task } from "@/types/task";

const SERVICES = [
    { id: 1, name: "Telegram" },
    { id: 2, name: "Email" },
    { id: 3, name: "Push" },
] as const;

interface NotificationFormProps {
    notification?: NotificationItem;
    onSuccess?: () => void;
}

export const NotificationForm = ({ notification, onSuccess }: NotificationFormProps) => {
    const [description, setDescription]     = useState(notification?.description || "");
    const [serviceId, setServiceId]         = useState<number>(notification?.serviceId ?? 1);
    const [scheduledAt, setScheduledAt]     = useState(notification?.scheduledAt || "");
    const [taskId, setTaskId]               = useState<string | undefined>(notification?.taskId ?? undefined);
    const [taskName, setTaskName]           = useState(notification?.task?.title || "");
    const [tasksList, setTasksList]         = useState<Task[]>(
        notification?.task ? [notification.task] : []
    );
    const [loading, setLoading]             = useState(false);
    const [error, setError]                 = useState("");
    const navigate = useNavigate();

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
        if (!description.trim()) { setError("Введите описание уведомления"); return; }
        if (!scheduledAt)        { setError("Укажите дату и время отправки"); return; }

        setError("");
        setLoading(true);
        try {
            if (notification) {
                await notificationApi.updateNotification(notification.notificationId, {
                    description,
                    scheduledAt,
                });
            } else {
                await notificationApi.createNotification({
                    serviceId,
                    description,
                    scheduledAt,
                    taskId: taskId ?? null,
                });
            }
            onSuccess?.();
            navigate(-1);
        } catch {
            setError("Ошибка сохранения уведомления");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto p-2 rounded-2xl space-y-6 pb-16">
            <h2 className="text-lg font-bold">
                {notification ? "Редактирование уведомления" : "Создание уведомления"}
            </h2>

            {error && <div className="text-sm text-red-500">{error}</div>}

            <div className="space-y-2">
                <Label>Описание</Label>
                <Textarea
                    placeholder="Текст уведомления..."
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </div>

            {/* Канал — только при создании, при редактировании менять нельзя */}
            {!notification && (
                <div className="space-y-2">
                    <Label>Канал отправки</Label>
                    <Tabs
                        value={String(serviceId)}
                        onValueChange={(v) => setServiceId(Number(v))}
                        className="w-full"
                    >
                        <TabsList className={`grid grid-cols-${SERVICES.length} w-full`}>
                            {SERVICES.map((s) => (
                                <TabsTrigger key={s.id} value={String(s.id)}>
                                    {s.name}
                                </TabsTrigger>
                            ))}
                        </TabsList>
                    </Tabs>
                </div>
            )}

            <div className="space-y-2">
                <Label>Дата и время отправки</Label>
                <DatePickerTime value={scheduledAt} onChange={setScheduledAt} />
            </div>

            <div className="space-y-2">
                <Label>Задача (необязательно)</Label>
                <Combobox
                    value={tasksList.find((t) => t.taskId === taskId) || null}
                    items={tasksList}
                    onValueChange={(task: Task | null) => {
                        setTaskId(task?.taskId);
                        setTaskName(task?.title || "");
                    }}
                >
                    <ComboboxInput
                        value={taskName}
                        placeholder="Поиск по задачам..."
                        onInput={(e) => setTaskName((e.target as HTMLInputElement).value)}
                    />
                    <ComboboxContent>
                        <ComboboxEmpty>Задача не найдена</ComboboxEmpty>
                        <ComboboxList>
                            {(item: Task) => (
                                <ComboboxItem key={item.taskId} value={item}>
                                    {item.title}
                                </ComboboxItem>
                            )}
                        </ComboboxList>
                    </ComboboxContent>
                </Combobox>
            </div>

            <Button onClick={handleSubmit} disabled={loading} className="w-full mt-2">
                {loading ? "Сохраняем..." : notification ? "Сохранить" : "Создать уведомление"}
            </Button>
        </div>
    );
};