import { useEffect, useState } from "react";
import { Box, Stack, Text, Spinner } from "@chakra-ui/react";
import { api } from "@/api/http";
import { useAuthStore } from "@/stores/authStore";

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
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!isAuth) return;

        const fetchTasks = async () => {
            setLoading(true);
            setError("");

            try {
                const res = await api.get<TasksResponse>("/tasks");
                if (!res.data.success) {
                    throw new Error("Ошибка при получении задач");
                }
                setTasks(res.data.data.tasks);
            } catch {
                setError("Сетевая ошибка");
            } finally {
                setLoading(false);
            }
        };

        fetchTasks();
    }, [isAuth]);

    if (!isAuth) return <Text>Пожалуйста, авторизуйтесь</Text>;
    if (loading) return <Spinner size="xl" />;
    if (error) return <Text color="red.500">{error}</Text>;
    if (tasks.length === 0) return <Text>Задач нет</Text>;

    return (
        <Stack gap={4}>
            {tasks.map((task) => (
                <Box
                    key={task.taskId}
                    p={4}
                    borderWidth="1px"
                    borderRadius="md"
                    _hover={{ shadow: "md" }}
                >
                    <Text fontWeight="bold">{task.title}</Text>
                    <Text fontSize="sm" color="gray.500">{task.projectName}</Text>
                    <Text>Status: {task.status} | Priority: {task.priority}</Text>
                    <Text fontSize="sm">Due: {new Date(task.dueDate).toLocaleString()}</Text>
                </Box>
            ))}
        </Stack>
    );
};
