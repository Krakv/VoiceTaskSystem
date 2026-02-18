import { useEffect, useState } from "react";
import { Box, Button, Stack, Text, Spinner } from "@chakra-ui/react";
import { taskApi } from "@/api/task.api";

interface Task {
    taskId: string;
    title: string;
    projectName: string;
    status: string;
    priority: string;
}

export const TaskManager = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const fetchTasks = async () => {
        setLoading(true);
        try {
            const res = await taskApi.getTasks();
            setTasks(res.data.data.tasks);
        } catch (err: any) {
            setError(err.message || "Ошибка загрузки");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchTasks();
    }, []);

    const handleDelete = async (taskId: string) => {
        try {
            await taskApi.deleteTask(taskId);
            setTasks((prev) => prev.filter((t) => t.taskId !== taskId));
        } catch (err: any) {
            setError(err.message || "Ошибка удаления");
        }
    };

    if (loading) return <Spinner size="xl" />;

    return (
        <Stack gap={4}>
            {error && <Text color="red.500">{error}</Text>}

            {tasks.length === 0 ? (
                <Text>Задач нет</Text>
            ) : (
                tasks.map((task) => (
                    <Box
                        key={task.taskId}
                        p={4}
                        borderWidth="1px"
                        borderRadius="md"
                        _hover={{ shadow: "md" }}
                        display="flex"
                        justifyContent="space-between"
                        alignItems="center"
                    >
                        <Box>
                            <Text fontWeight="bold">{task.title}</Text>
                            <Text fontSize="sm">
                                {task.projectName || "Без проекта"} | {task.status} | {task.priority}
                            </Text>
                        </Box>
                        <Button size="sm" colorScheme="red" onClick={() => handleDelete(task.taskId)}>
                            Удалить
                        </Button>
                    </Box>
                ))
            )}
        </Stack>
    );
};
