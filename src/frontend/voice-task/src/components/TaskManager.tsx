import { useEffect, useState } from "react";
import { taskApi } from "@/api/task.api";
import {Button} from "@/components/ui/button.tsx";
import {useNavigate} from "react-router-dom";
import {Plus} from "lucide-react";
import type {Task} from "@/types/task";
import {TaskCard} from "@/components/TaskCard.tsx";
import {TaskSheet} from "@/components/TaskSheet.tsx";

export const TaskManager = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [error, setError] = useState("");
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [open, setOpen] = useState(false);
    const navigate = useNavigate();

    const fetchTasks = async () => {
        try {
            const res = await taskApi.getTasks();
            setTasks(res.data.data.tasks);
        } catch (err: any) {
            setError(err.message || "Ошибка загрузки");
        }
    };

    const handleOpen = async (task: Task) => {
        const { data } = await taskApi.getTaskById(task.taskId);
        console.log(data.data);
        setSelectedTask(data.data);
        setOpen(true);
    };

    const handleToggle = async (task: Task) => {
        await taskApi.updateTask(task.taskId, {
            status: task.status === "done" ? "inprogress" : "done",
        });
    };

    useEffect(() => {
        fetchTasks();
    }, []);

    const handleCreate = () => {
        navigate("/create");
        console.log("Создать новую задачу");
    };

    return (
        <div className="flex flex-col gap-4">
            {error && <div color="red.500">{error}</div>}

            {tasks.length === 0 ? (
                <div>Задач нет</div>
            ) : (
                tasks.map((task) => (
                    <TaskCard key={task.taskId} task={task} onOpen={handleOpen} onToggle={handleToggle}/>
                ))
            )}

            <TaskSheet task={selectedTask} open={open} onOpenChange={setOpen} />

            <Button
                onClick={handleCreate}
                className="sticky bottom-4 right-4 rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
            >
                <Plus className="w-6 h-6" />
            </Button>
        </div>
    );
};
