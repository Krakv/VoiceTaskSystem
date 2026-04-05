import { api } from "@/api/http";
import type {TaskPriority, TaskStatus} from "@/types/task.ts";

export interface TaskCreateDto {
    title: string;
    projectName?: string;
    description?: string;
    status?: TaskStatus;
    priority?: TaskPriority;
    dueDate?: string;
    parentTaskId?: string | null;
}

export interface TaskUpdateDto {
    title?: string;
    projectName?: string;
    description?: string;
    status?: TaskStatus;
    priority?: TaskPriority;
    dueDate?: string;
    parentTaskId?: string | null;
}

export const taskApi = {
    getTasks: () => api.get("/tasks"),
    createTask: (data: TaskCreateDto) => api.post("/tasks", data),
    deleteTask: (taskId: string) => api.delete(`/tasks/${taskId}`),
    getTaskById: (taskId: string) => api.get(`/tasks/${taskId}`),
    updateTask: (taskId: string, data: TaskUpdateDto) => api.patch(`/tasks/${taskId}`, data),
};
