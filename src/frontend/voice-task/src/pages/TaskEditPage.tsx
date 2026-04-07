import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { taskApi } from "@/api/task.api";
import { TaskForm } from "@/components/TaskForm";
import type { Task } from "@/types/task";

export const TaskEditPage = () => {
    const { taskId } = useParams<{ taskId: string }>();
    const [task, setTask] = useState<Task | undefined>(undefined);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!taskId) return;
        taskApi.getTaskById(taskId)
            .then((res) => {
                setTask(res.data.data);
            })
            .finally(() => setLoading(false));
    }, [taskId]);


    return (
        <>
            {loading ? (<div>Загрузка...</div>) : <TaskForm task={task} /> }
        </>
    );
};