import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import type { FC } from "react";
import type {RuleItem, RuleAction, CreateCalendarEventAction, SetFieldAction} from "@/types/rule";

interface Props {
    rule: RuleItem;
    onOpen: (rule: RuleItem) => void;
    onToggle: (rule: RuleItem) => void;
}

const eventLabels: Record<string, string> = {
    taskCreated: "Создание",
    taskUpdated: "Обновление",
    taskDeleted: "Удаление",
    taskCompleted: "Завершение",
    taskOverdue: "Просрочка",
};

function renderActionSummary(action: RuleAction) {
    switch (action.type) {
        case "SET_FIELD": {
            const a = action as SetFieldAction;
            return `Изменить ${a.field} → ${a.value}`;
        }

        case "CREATE_NOTIFICATION": {
            return `Уведомление`;
        }

        case "CREATE_CALENDAR_EVENT": {
            const a = action as CreateCalendarEventAction;
            return `Событие (${a.durationMinutes} мин)`;
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
        space-y-3
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
                        {eventLabels[rule.ruleEvent]}
                    </div>

                    {/* Условия */}
                    {rule.condition.conditions.length > 0 && (
                        <div className="text-xs text-muted-foreground">
                            {rule.condition.conditions.length} условий (
                            {rule.condition.operator})
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
                            className={
                                rule.isActive
                                    ? "bg-green-100 text-green-700"
                                    : "bg-gray-100 text-gray-500"
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