import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import type { FC } from "react";
import type { RuleItem, RuleAction, CreateCalendarEventAction, SetFieldAction, CreateNotificationAction } from "@/types/rule";

interface Props {
    rule: RuleItem;
    onOpen: (rule: RuleItem) => void;
    onToggle: (rule: RuleItem) => void;
}

const eventLabels: Record<string, string> = {
    taskCreated: "Задача создана",
    taskUpdated: "Задача обновлена",
    taskDeleted: "Задача удалена",
    taskCompleted: "Задача завершена",
    taskOverdue: "Задача просрочена",
};

const fieldLabels: Record<string, string> = {
    priority: "Приоритет",
    status: "Статус",
    dueDate: "Дедлайн",
    title: "Заголовок",
    description: "Описание",
    projectName: "Проект",
};

const valueLabels: Record<string, string> = {
    low: "низкий",
    medium: "средний",
    high: "высокий",
    new: "новая",
    inProgress: "в работе",
    done: "готово",
    canceled: "отменена",
};

function renderActionSummary(action: RuleAction) {
    switch (action.type) {
        case "SET_FIELD": {
            const a = action as SetFieldAction;
            const field = fieldLabels[a.field] || a.field;
            const value = valueLabels[a.value] || a.value;
            return `${field} → ${value}`;
        }

        case "CREATE_NOTIFICATION": {
            const a = action as CreateNotificationAction;
            const service = a.serviceId === "telegram" ? "ТГ" : "Email";
            return `Уведомление (${service})`;
        }

        case "CREATE_CALENDAR_EVENT": {
            const a = action as CreateCalendarEventAction;
            return `Календарь (${a.durationMinutes} мин)`;
        }

        default:
            return action.type;
    }
}

export const RuleCard: FC<Props> = ({ rule, onOpen, onToggle }) => {
    return (
        <div
            onClick={() => onOpen(rule)}
            className="
                p-4 rounded-2xl border bg-white
                active:scale-[0.98] transition
                space-y-3 cursor-pointer
            "
        >
            <div className="flex gap-3">
                <div
                    onClick={(e) => {
                        e.stopPropagation();
                        onToggle(rule);
                    }}
                >
                    <Checkbox checked={rule.isActive} />
                </div>

                <div className="flex-1 space-y-2">
                    {/* Заголовок */}
                    <div className="text-sm font-medium leading-snug">
                        {eventLabels[rule.ruleEvent] || rule.ruleEvent}
                    </div>

                    {/* Условия */}
                    {rule.condition.conditions.length > 0 && (
                        <div className="text-xs text-muted-foreground">
                            {rule.condition.conditions.length} {rule.condition.conditions.length === 1 ? 'условие' : 'условий'} (
                            {rule.condition.operator === "and" ? "И" : "ИЛИ"})
                        </div>
                    )}

                    {/* Действия */}
                    <div className="flex flex-wrap gap-2">
                        {rule.action.slice(0, 2).map((a, i) => (
                            <Badge key={i} variant="secondary">
                                {renderActionSummary(a)}
                            </Badge>
                        ))}

                        {rule.action.length > 2 && (
                            <Badge variant="outline">
                                +{rule.action.length - 2}
                            </Badge>
                        )}
                    </div>

                    {/* Статус */}
                    <div className="flex gap-2">
                        <Badge
                            variant="secondary"
                            className={
                                rule.isActive
                                    ? "bg-green-100 text-green-700 hover:bg-green-100 border-none"
                                    : "bg-gray-100 text-gray-500 hover:bg-gray-100 border-none"
                            }
                        >
                            {rule.isActive ? "Активно" : "Выключено"}
                        </Badge>
                    </div>
                </div>
            </div>
        </div>
    );
};