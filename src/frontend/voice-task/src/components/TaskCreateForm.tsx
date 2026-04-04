import { useState } from "react";
import { taskApi, type TaskCreateDto } from "@/api/task.api";
import { Card } from "@/components/ui/card.tsx";
import { Input } from "@/components/ui/input.tsx";
import { Button } from "@/components/ui/button.tsx";

interface TaskFormProps {
    onSuccess?: () => void;
}

export const TaskCreateForm = ({ onSuccess }: TaskFormProps) => {
    const [title, setTitle] = useState("");
    const [projectName, setProjectName] = useState("");
    const [priority, setPriority] = useState<"Low" | "Medium" | "High">("Low");
    const [status, setStatus] = useState<"New" | "InProgress" | "Done" | "Canceled">("New");
    const [error, setError] = useState("");

    const handleSubmit = async () => {
        if (!title) {
            setError("Введите заголовок задачи");
            return;
        }
        setError("");

        const data: TaskCreateDto = {
            title,
            projectName: projectName || undefined,
            description: "",
            priority,
            status
        };

        try {
            await taskApi.createTask(data);
            setTitle("");
            setProjectName("");
            setPriority("Low");
            setStatus("New");
            onSuccess?.();
        } catch (err) {
            console.error(err);
            setError("Ошибка создания задачи");
        }
    };

    return (
        <Card className="max-w-md mx-auto p-6">
            <div className="flex flex-col gap-4">
                {error && <div className="text-red-500 text-sm">{error}</div>}

                <Input
                    placeholder="Заголовок задачи"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />
                <Input
                    placeholder="Проект (необязательно)"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                />

                <div className="flex gap-2">
                    <select
                        value={priority}
                        onChange={(e) => setPriority(e.target.value as "Low" | "Medium" | "High")}
                        className="flex-1 border border-gray-300 rounded-md p-2 text-sm focus:ring-1 focus:ring-blue-500 focus:outline-none"
                    >
                        <option value="Low">Низкий</option>
                        <option value="Medium">Средний</option>
                        <option value="High">Высокий</option>
                    </select>

                    <select
                        value={status}
                        onChange={(e) => setStatus(e.target.value as "New" | "InProgress" | "Done" | "Canceled")}
                        className="flex-1 border border-gray-300 rounded-md p-2 text-sm focus:ring-1 focus:ring-blue-500 focus:outline-none"
                    >
                        <option value="New">Новая</option>
                        <option value="InProgress">В работе</option>
                        <option value="Done">Выполнена</option>
                        <option value="Canceled">Отменена</option>
                    </select>
                </div>

                <Button onClick={handleSubmit} className="mt-2 w-full">
                    Создать задачу
                </Button>
            </div>
        </Card>
    );
};