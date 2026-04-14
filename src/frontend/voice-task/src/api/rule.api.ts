import {api} from "@/api/http.ts";
import type {
    ConditionGroup,
    RuleAction,
    RuleEvent
} from "@/types/rule.ts";

export interface RuleCreateDto {
    ruleEvent: RuleEvent;
    conditions: ConditionGroup;
    actions: RuleAction[];
}

export const ruleApi = {
    getRules: () => api.get("/rules"),
    createRule: (data: RuleCreateDto) => api.post("/rules", data),
    deleteRule: (ruleId: string) => api.delete(`/rules/${ruleId}`),
    getRuleById: (ruleId: string) => api.get(`/rules/${ruleId}`),
    updateRule: (ruleId: string, data: RuleCreateDto) => api.put(`/rules/${ruleId}`, data),
    toggleRule: (ruleId: string) => api.patch(`/rules/${ruleId}`)
};