import { api } from "@/api/http";

export interface TaskCreateDto {
    title: string;
    projectName?: string;
    description: string;
    status?: "new" | "in_progress" | "done" | "canceled";
    priority?: "low" | "medium" | "high";
    dueDate?: string;
    tags: string;
    parentTaskId?: string | null;
}

export const taskApi = {
    getTasks: () => api.get("/tasks"),
    createTask: (data: TaskCreateDto) => api.post("/tasks", data),
    deleteTask: (taskId: string) => api.delete(`/tasks/${taskId}`),
};
