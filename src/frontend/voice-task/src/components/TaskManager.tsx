import { useEffect, useState } from "react";
import { taskApi } from "@/api/task.api";
import {Card} from "@/components/ui/card.tsx";
import {Button} from "@/components/ui/button.tsx";
import {useNavigate} from "react-router-dom";
import {Plus} from "lucide-react";

interface Task {
    taskId: string;
    title: string;
    projectName: string;
    status: string;
    priority: string;
}

export const TaskManager = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const fetchTasks = async () => {
        try {
            const res = await taskApi.getTasks();
            setTasks(res.data.data.tasks);
        } catch (err: any) {
            setError(err.message || "Ошибка загрузки");
        }
    };

    useEffect(() => {
        fetchTasks();
    }, []);

    const handleCreate = () => {
        navigate("/create");
        console.log("Создать новую задачу");
    };

    const handleDelete = async (taskId: string) => {
        try {
            await taskApi.deleteTask(taskId);
            setTasks((prev) => prev.filter((t) => t.taskId !== taskId));
        } catch (err: any) {
            setError(err.message || "Ошибка удаления");
        }
    };

    return (
        <div className="flex flex-col gap-4">
            {error && <div color="red.500">{error}</div>}

            {tasks.length === 0 ? (
                <div>Задач нет</div>
            ) : (
                tasks.map((task) => (
                    <Card
                        key={task.taskId}
                        className="p-4 border rounded-md flex justify-between items-center hover:shadow-md transition-shadow"
                    >
                        <Card>
                            <div>{task.title}</div>
                            <div>
                                {task.projectName || "Без проекта"} | {task.status} | {task.priority}
                            </div>
                        </Card>
                        <Button size="sm" onClick={() => handleDelete(task.taskId)}>
                            Удалить
                        </Button>
                    </Card>
                ))
            )}

            <Button
                onClick={handleCreate}
                className="sticky bottom-4 right-4 rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-blue-600 text-white hover:bg-blue-700"
            >
                <Plus className="w-6 h-6" />
            </Button>
        </div>
    );
};
