import { useState } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { DatePickerTime } from "@/components/DatePickerTime";
import { commandRequestApi } from "@/api/commandRequest.api";

import type { TaskCreatePayload } from "@/types/commandRequest";
import type { TaskPriority, TaskStatus } from "@/types/task";

interface Props {
    commandRequestId: string;
    initialData: TaskCreatePayload;
    onSuccess?: () => void;
}

export const CommandRequestForm = ({
                                       commandRequestId,
                                       initialData,
                                       onSuccess,
                                   }: Props) => {

    const [title, setTitle] = useState(initialData.title || "");
    const [description, setDescription] = useState(initialData.description || "");
    const [projectName, setProjectName] = useState(initialData.projectName || "");
    const [dueDate, setDueDate] = useState(initialData.dueDate || "");
    const [priority, setPriority] = useState<TaskPriority>(
        (initialData.priority as TaskPriority) || "low"
    );
    const [status, setStatus] = useState<TaskStatus>(
        (initialData.status as TaskStatus) || "new"
    );

    const [parentTaskId, setParentTaskId] = useState(initialData.parentTask?.taskId);
    const [parentTaskName, setParentTaskName] = useState(initialData.parentTask?.title || "");

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
            <h2 className="text-lg font-bold">Редактирование команды</h2>

            {error && <div className="text-red-500 text-sm">{error}</div>}

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

            <Button onClick={handleSubmit} disabled={loading} className="w-full">
                {loading ? "Сохраняем..." : "Подтвердить"}
            </Button>
        </div>
    );
};