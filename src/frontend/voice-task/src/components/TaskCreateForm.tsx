import { useState } from "react";
import { taskApi, type TaskCreateDto } from "@/api/task.api";
import {Card} from "@/components/ui/card.tsx";
import {Input} from "@/components/ui/input.tsx";
import {Button} from "@/components/ui/button.tsx";

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
        <Card >
            <div className="flex flex-col gap-4">
                {error && <div>{error}</div>}

                <Input
                    placeholder="Заголовок"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                />
                <Input
                    placeholder="Проект"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                />
                <select
                    value={priority}
                    onChange={(e) => setPriority(e.target.value as "Low" | "Medium" | "High")}
                    style={{
                        padding: "0.5rem",
                        borderRadius: 4,
                        border: "1px solid #CBD5E0",
                    }}
                >
                    <option value="Low">Low</option>
                    <option value="Medium">Medium</option>
                    <option value="High">High</option>
                </select>

                <select
                    value={status}
                    onChange={(e) => setStatus(e.target.value as "New" | "InProgress" | "Done" | "Canceled")}
                    style={{
                        padding: "0.5rem",
                        borderRadius: 4,
                        border: "1px solid #CBD5E0",
                    }}
                >
                    <option value="New">New</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Done">Done</option>
                    <option value="Canceled">Canceled</option>
                </select>

                <Button onClick={handleSubmit} >
                    Создать задачу
                </Button>
            </div>
        </Card>
    );
};
