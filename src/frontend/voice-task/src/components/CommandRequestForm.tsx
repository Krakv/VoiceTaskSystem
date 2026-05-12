import { useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { DatePickerTime } from "@/components/DatePickerTime";
import { Badge } from "@/components/ui/badge";
import { commandRequestApi } from "@/api/commandRequest.api";

import type { TaskCreatePayload, TaskUpdatePayload } from "@/types/commandRequest";
import type { TaskPriority, TaskStatus } from "@/types/task";

interface Props {
    commandRequestId: string;
    initialData: TaskUpdatePayload | TaskCreatePayload;
    onSuccess?: () => void;
}

export const CommandRequestForm = ({
                                       commandRequestId,
                                       initialData,
                                       onSuccess,
                                   }: Props) => {
    const isUpdatePayload = (data: TaskUpdatePayload | TaskCreatePayload): data is TaskUpdatePayload => {
        return "tasks" in data && Array.isArray(data.tasks);
    };

    const getInitialValue = <T extends string>(field: string, fallback: T): T | string => {
        const val = (initialData as any)[field];
        if (val !== null && val !== undefined && val !== "") return val;

        if (isUpdatePayload(initialData) && initialData.tasks.length > 0) {
            const taskValue = (initialData.tasks[0] as any)[field];
            if (taskValue !== null && taskValue !== undefined && taskValue !== "") {
                return taskValue;
            }
        }

        return fallback;
    };

    const [title, setTitle] = useState<string>(getInitialValue("title", ""));
    const [description, setDescription] = useState<string>(getInitialValue("description", ""));
    const [projectName, setProjectName] = useState<string>(getInitialValue("projectName", ""));
    const [dueDate, setDueDate] = useState<string>(getInitialValue("dueDate", ""));

    const [priority, setPriority] = useState<TaskPriority>(
        getInitialValue("priority", "low") as TaskPriority
    );
    const [status, setStatus] = useState<TaskStatus>(
        getInitialValue("status", "new") as TaskStatus
    );

    // Логика для родительской задачи
    const getInitialParent = () => {
        if ("parentTask" in initialData && initialData.parentTask) return initialData.parentTask;
        if (isUpdatePayload(initialData) && initialData.tasks[0]?.parentTask) return initialData.tasks[0].parentTask;
        return null;
    };

    const initialParent = getInitialParent();
    const [parentTaskId, setParentTaskId] = useState<string | undefined>(initialParent?.taskId);
    const [parentTaskName, setParentTaskName] = useState<string>(initialParent?.title || "");

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const handleSubmit = async () => {
        if (!title.trim()) {
            setError("Введите заголовок");
            return;
        }

        setLoading(true);
        setError("");

        try {
            const payload: TaskCreatePayload = {
                title,
                description: description || undefined,
                projectName: projectName || undefined,
                dueDate: dueDate || undefined,
                priority,
                status,
                parentTaskId,
            };

            await commandRequestApi.patchVoiceTask(commandRequestId, payload);
            onSuccess?.();
        } catch {
            setError("Ошибка сохранения");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto p-4 space-y-5">
            <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold">Редактирование</h2>
                {isUpdatePayload(initialData) && initialData.tasks.length > 0 && (
                    <Badge variant="outline" className="text-[10px] text-muted-foreground font-mono">
                        ID: {initialData.tasks[0].taskId.slice(0, 8)}
                    </Badge>
                )}
            </div>

            {error && <div className="text-red-500 text-sm font-medium">{error}</div>}

            <div className="space-y-2">
                <Label>Название</Label>
                <Input value={title} onChange={(e) => setTitle(e.target.value)} />
            </div>

            <div className="space-y-2">
                <Label>Описание</Label>
                <Textarea value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>

            <div className="space-y-2">
                <Label>Проект</Label>
                <Input value={projectName} onChange={(e) => setProjectName(e.target.value)} />
            </div>

            <div className="space-y-2">
                <Label>Родительская задача</Label>
                <Input
                    value={parentTaskName}
                    onChange={(e) => {
                        setParentTaskName(e.target.value);
                        setParentTaskId(undefined);
                    }}
                    placeholder="Введите название"
                />
            </div>

            <div className="space-y-2">
                <Label>Дедлайн</Label>
                <DatePickerTime value={dueDate} onChange={setDueDate} />
            </div>

            <div className="space-y-2">
                <Label>Приоритет</Label>
                <Tabs value={priority} onValueChange={(v) => setPriority(v as TaskPriority)}>
                    <TabsList className="grid grid-cols-3 w-full">
                        <TabsTrigger value="low">Низкий</TabsTrigger>
                        <TabsTrigger value="medium">Средний</TabsTrigger>
                        <TabsTrigger value="high">Высокий</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <div className="space-y-2">
                <Label>Статус</Label>
                <Tabs value={status} onValueChange={(v) => setStatus(v as TaskStatus)}>
                    <TabsList className="grid grid-cols-4 w-full">
                        <TabsTrigger value="new">Новая</TabsTrigger>
                        <TabsTrigger value="inProgress">В работе</TabsTrigger>
                        <TabsTrigger value="done">Готово</TabsTrigger>
                        <TabsTrigger value="canceled">Отмена</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <Button onClick={handleSubmit} disabled={loading} className="w-full h-11 font-bold shadow-md">
                {loading ? "Сохраняем..." : "Подтвердить изменения"}
            </Button>
        </div>
    );
};