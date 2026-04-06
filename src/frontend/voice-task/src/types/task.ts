export type TaskStatus = "new" | "inProgress" | "done" | "canceled";
export type TaskPriority = "low" | "medium" | "high";

export interface Task {
    taskId: string;
    title: string;
    description?: string;
    projectName?: string;
    status: TaskStatus;
    priority: TaskPriority;
    dueDate?: string;
    parentTask?: ShortTaskInfo | null;
    childrenTasks?: ShortTaskInfo[] | null;
}

export interface ShortTaskInfo {
    taskId: string;
    title: string;
    status: TaskStatus;
    priority: TaskPriority;
    dueDate?: string;
}