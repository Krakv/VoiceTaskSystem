import {useCallback, useEffect, useState} from "react";
import { taskApi, GetTasksQuery } from "@/api/task.api";
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
            const res = await taskApi.getTasks(new GetTasksQuery());
            setTasks(res.data.data.tasks);
        } catch (err: any) {
            setError(err.message || "Ошибка загрузки");
        }
    };

    const handleOpen = async (taskId: string) => {
        const { data } = await taskApi.getTaskById(taskId);
        setSelectedTask(data.data);
        setOpen(true);
    };

    const handleToggle = async (task: Task) => {
        await taskApi.updateTask(task.taskId, {
            status: task.status === "done" ? "inProgress" : "done",
        });
    };

    const handleEdit = useCallback((taskId: string) => {
        navigate(`/tasks/${taskId}/edit`);
    }, [navigate]);

    useEffect(() => {
        fetchTasks();
    }, []);

    const handleCreate = () => {
        navigate("/create");
    };

    return (
        <div className="flex flex-col gap-4">
            {error && <div color="red.500">{error}</div>}

            {tasks.length === 0 ? (
                <div>Задач нет</div>
            ) : (
                tasks.map((task) => (
                    <TaskCard key={task.taskId} task={task} onOpen={t => handleOpen(t.taskId)} onToggle={handleToggle}/>
                ))
            )}

            <TaskSheet
                task={selectedTask}
                open={open}
                onOpenChange={setOpen}
                onOpenTask={async (t) => {
                    setOpen(false);
                    await handleOpen(t);
                }}
                onToggleSubtask={async (taskId, status) => {
                    await taskApi.updateTask(taskId, {
                        status: status === "done" ? "inProgress" : "done",
                    });

                    await handleOpen(selectedTask!.taskId);
                }}
                onEdit={handleEdit}
                onDelete={async (taskId) => {
                    await taskApi.deleteTask(taskId);
                    setOpen(false);
                    await fetchTasks();
                }}
            />

            <Button
                onClick={handleCreate}
                className="sticky bottom-4 right-4 rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
            >
                <Plus className="w-6 h-6" />
            </Button>
        </div>
    );
};
