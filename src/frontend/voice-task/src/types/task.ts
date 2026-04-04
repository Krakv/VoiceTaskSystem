export type TaskStatus = "new" | "inprogress" | "done" | "canceled";
export type TaskPriority = "low" | "medium" | "high";

export interface Task {
    taskId: string;
    title: string;
    description?: string;
    projectName?: string;
    status: TaskStatus;
    priority: TaskPriority;
    dueDate?: string;
    parentTaskId?: string | null;
    subtasks?: Task[];
}