import { useState } from "react";
import { taskApi, type TaskCreateDto } from "@/api/task.api";

import {Card, CardHeader} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";

import type { TaskPriority, TaskStatus } from "@/types/task";
import { DatePickerTime } from "@/components/DatePickerTime";
import { useNavigate } from "react-router-dom";

interface TaskFormProps {
    onSuccess?: () => void;
}

export const TaskCreateForm = ({ onSuccess }: TaskFormProps) => {
    const [title, setTitle] = useState("");
    const [projectName, setProjectName] = useState("");
    const [description, setDescription] = useState("");
    const [dueDate, setDueDate] = useState("");

    const [priority, setPriority] = useState<TaskPriority>("low");
    const [status, setStatus] = useState<TaskStatus>("new");

    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const navigate = useNavigate();

    const handleSubmit = async () => {
        if (!title.trim()) {
            setError("Введите заголовок задачи");
            return;
        }

        setError("");
        setLoading(true);

        const data: TaskCreateDto = {
            title,
            projectName: projectName || undefined,
            description: description || undefined,
            dueDate: dueDate || undefined,
            priority,
            status,
        };

        try {
            await taskApi.createTask(data);

            setTitle("");
            setProjectName("");
            setDescription("");
            setDueDate("");
            setPriority("low");
            setStatus("new");

            onSuccess?.();
        } catch (err) {
            console.error(err);
            setError("Ошибка создания задачи");
        } finally {
            setLoading(false);
            navigate(-1);
        }
    };

    return (
        <Card className="max-w-md mx-auto p-4 rounded-2xl">
            <CardHeader className="text-base font-bold">Создание задачи</CardHeader>
            {error && <div className="text-sm text-red-500">{error}</div>}

            <div className="space-y-2">
                <Label className="text-base font-semibold">Заголовок</Label>
                <Input
                    placeholder="Например: Сделать отчёт"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />
            </div>

            <div className="space-y-2">
                <Label className="text-base font-semibold">Описание</Label>
                <Textarea
                    placeholder="Описание задачи..."
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </div>

            <div className="space-y-2">
                <Label className="text-base font-semibold">Проект</Label>
                <Input
                    placeholder="Необязательно"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                />
            </div>

            <div className="space-y-2">
                <Label className="text-base font-semibold">Дедлайн</Label>
                <DatePickerTime value={dueDate} onChange={setDueDate} />
            </div>

            <div className="space-y-2">
                <Label className="text-base font-semibold">Приоритет</Label>
                <Tabs value={priority} onValueChange={value => setPriority(value as TaskPriority)} className="w-full">
                    <TabsList className="grid grid-cols-3 w-full">
                        <TabsTrigger value="low" className="text-green-600">Низкий</TabsTrigger>
                        <TabsTrigger value="medium" className="text-yellow-600">Средний</TabsTrigger>
                        <TabsTrigger value="high" className="text-red-600">Высокий</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <div className="space-y-2">
                <Label className="text-base font-semibold">Статус</Label>
                <Tabs value={status} onValueChange={value => setStatus(value as TaskStatus)} className="w-full">
                    <TabsList className="grid grid-cols-3 w-full">
                        <TabsTrigger value="new" className="text-gray-700">Новая</TabsTrigger>
                        <TabsTrigger value="inprogress" className="text-blue-600">В работе</TabsTrigger>
                        <TabsTrigger value="done" className="text-green-700">Готово</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <Button
                onClick={handleSubmit}
                disabled={loading}
                className="w-full mt-4"
            >
                {loading ? "Создание..." : "Создать задачу"}
            </Button>
        </Card>
    );
};