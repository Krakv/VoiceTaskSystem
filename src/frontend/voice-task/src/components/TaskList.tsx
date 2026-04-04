import { useEffect, useState } from "react";
import { api } from "@/api/http";
import { useAuthStore } from "@/stores/authStore";
import {Card} from "@/components/ui/card.tsx";

interface Task {
    taskId: string;
    projectName: string;
    title: string;
    description: string;
    status: string;
    priority: string;
    dueDate: string;
}

interface TasksResponse {
    data: {
        tasks: Task[];
    };
    success: boolean;
    meta: {
        requestId: string;
        timestamp: string;
    };
}

export const TaskList = () => {
    const { isAuth } = useAuthStore();
    const [tasks, setTasks] = useState<Task[]>([]);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!isAuth) return;

        const fetchTasks = async () => {
            setError("");

            try {
                const res = await api.get<TasksResponse>("/tasks");
                if (!res.data.success) {
                    throw new Error("Ошибка при получении задач");
                }
                setTasks(res.data.data.tasks);
            } catch {
                setError("Сетевая ошибка");
            }
        };

        fetchTasks();
    }, [isAuth]);

    if (!isAuth) return <div>Пожалуйста, авторизуйтесь</div>;
    if (error) return <div color="red.500">{error}</div>;
    if (tasks.length === 0) return <div>Задач нет</div>;

    return (
        <div className="flex flex-col gap-4">
            {tasks.map((task) => (
                <Card
                    key={task.taskId}
                    className="p-4 border rounded-md hover:shadow-md transition-shadow"
                >
                    <div className="font-bold">{task.title}</div>
                    <div className="text-sm text-muted-foreground">
                        {task.projectName}
                    </div>
                    <div className="text-sm">
                        Status: {task.status} | Priority: {task.priority}
                    </div>
                    <div className="text-sm text-muted-foreground">
                        Due: {new Date(task.dueDate).toLocaleString()}
                    </div>
                </Card>
            ))}
        </div>
    );
};
