import type {Task} from "@/types/task.ts";

export type CommandIntent =
    | "taskCreate"
    | "taskUpdate"
    | "taskDelete"
    | "taskQuery"
    | "unknown"
    | "ambiguous";

export interface TaskCreatePayload {
    title: string;
    description?: string;
    status: string;
    priority: string;
    dueDate?: string;
    message?: string;
    projectName?: string;
    parentTaskId?: string;
    parentTask?: {
        taskId: string;
        title: string;
    };
    confirmationRequired?: boolean;
}

// TASK_UPDATE treated like TASK_CREATE per latest backend
export type TaskUpdatePayload = TaskCreatePayload;

export interface TaskDeletePayload {
    confirmationRequired?: boolean;
}

export interface TaskQueryPayload {
    tasks: Task[];
}

export interface UnknownPayload {
    message?: string;
}

export type AnyPayload =
    | TaskCreatePayload
    | TaskUpdatePayload
    | TaskDeletePayload
    | TaskQueryPayload
    | UnknownPayload;

export interface VoiceStatusData {
    intent: CommandIntent;
    payload: AnyPayload;
}