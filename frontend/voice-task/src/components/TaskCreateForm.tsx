import { useState } from "react";
import { Box, Button, Input, Stack, Text } from "@chakra-ui/react";
import { taskApi, type TaskCreateDto } from "@/api/task.api";

interface TaskFormProps {
    onSuccess?: () => void;
}

export const TaskCreateForm = ({ onSuccess }: TaskFormProps) => {
    const [title, setTitle] = useState("");
    const [projectName, setProjectName] = useState("");
    const [priority, setPriority] = useState<"low" | "medium" | "high">("low");
    const [status, setStatus] = useState<"new" | "in_progress" | "done">("new");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async () => {
        if (!title) {
            setError("Введите заголовок задачи");
            return;
        }
        setError("");
        setLoading(true);

        const data: TaskCreateDto = {
            title,
            projectName: projectName || undefined,
            description: "",
            priority,
            status,
            tags: ""
        };

        try {
            await taskApi.createTask(data);
            setTitle("");
            setProjectName("");
            setPriority("low");
            setStatus("new");
            onSuccess?.();
        } catch (err) {
            console.error(err);
            setError("Ошибка создания задачи");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box p={4} borderWidth="1px" borderRadius="md" width="540px">
            <Stack gap={3}>
                {error && <Text color="red.500">{error}</Text>}

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
                    onChange={(e) => setPriority(e.target.value as "low" | "medium" | "high")}
                    style={{
                        padding: "0.5rem",
                        borderRadius: 4,
                        border: "1px solid #CBD5E0",
                    }}
                >
                    <option value="low">Low</option>
                    <option value="medium">Medium</option>
                    <option value="high">High</option>
                </select>

                <select
                    value={priority}
                    onChange={(e) => setPriority(e.target.value as "low" | "medium" | "high")}
                    style={{
                        padding: "0.5rem",
                        borderRadius: 4,
                        border: "1px solid #CBD5E0",
                    }}
                >
                    <option value="new">New</option>
                    <option value="in_progress">In Progress</option>
                    <option value="done">Done</option>
                </select>

                <Button colorScheme="teal" onClick={handleSubmit} loading={loading}>
                    Создать задачу
                </Button>
            </Stack>
        </Box>
    );
};
