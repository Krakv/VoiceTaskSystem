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

export interface TaskUpdatePayload {
    tasks: Task[];
    title: string | null;
    description: string | null;
    status: string | null;
    priority: string | null;
    dueDate: string | null;
    projectName: string | null;
    confirmationRequired: boolean;
}

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