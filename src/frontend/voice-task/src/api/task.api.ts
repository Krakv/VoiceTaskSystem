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

export class GetTasksQuery {
    query?: string;
    status?: string;
    priority?: string;
    sortBy?: string;
    sortOrder?: string;
    limit: string;
    page: string;

    constructor({
                    query,
                    status,
                    priority,
                    sortBy,
                    sortOrder,
                    limit = "20",
                    page = "0",
                }: Partial<GetTasksQuery> = {}) {
        this.query = query;
        this.status = status;
        this.priority = priority;
        this.sortBy = sortBy;
        this.sortOrder = sortOrder;
        this.limit = limit;
        this.page = page;
    }
}

export class GetProjectsQuery {
    projectName?: string;
    limit: string;
    page: string;

    constructor({
                    projectName,
                    limit = "20",
                    page = "0",
                }: Partial<GetProjectsQuery> = {}) {
        this.projectName = projectName;
        this.limit = limit;
        this.page = page;
    }
}

export const taskApi = {
    getTasks: (data: GetTasksQuery) => api.get("/tasks", { params: data }),
    createTask: (data: TaskCreateDto) => api.post("/tasks", data),
    deleteTask: (taskId: string) => api.delete(`/tasks/${taskId}`),
    getTaskById: (taskId: string) => api.get(`/tasks/${taskId}`),
    updateTask: (taskId: string, data: TaskUpdateDto) => api.patch(`/tasks/${taskId}`, data),
    getProjects: (data: GetProjectsQuery) => api.get(`/tasks/projects`, { params: data }),
};
