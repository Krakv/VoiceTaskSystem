export type RuleEvent = "taskCreated" | "taskUpdated" | "taskDeleted" | "taskCompleted" | "taskOverdue";
export type ActionType = "SET_FIELD" | "CREATE_NOTIFICATION" | "CREATE_CALENDAR_EVENT";
export type LogicalOperator = "and" | "or";
export type ComparisonOperator = "eq" | "neq" | "gt" | "lt";

export interface Condition {
    field: string;
    operator: ComparisonOperator;
    value: string;
}

export interface ConditionGroup {
    operator: LogicalOperator;
    conditions: Condition[];
}

export interface RuleAction {
    type: ActionType;
}

export interface SetFieldAction extends RuleAction {
    type: "SET_FIELD";
    field: string;
    value: string;
}

export interface CreateNotificationAction extends RuleAction {
    type: "CREATE_NOTIFICATION";
    description: string;
    offsetMinutes: number;
}

export interface CreateCalendarEventAction extends RuleAction {
    type: "CREATE_CALENDAR_EVENT";
    durationMinutes: number;
    location: string;
}

export interface RuleItem {
    ruleId: string;
    ruleEvent: RuleEvent;
    condition: ConditionGroup;
    action: RuleAction[];
    isActive: boolean;
}