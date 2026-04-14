import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { ruleApi, type RuleCreateDto } from "@/api/rule.api";
import type {
    ActionType,
    ComparisonOperator,
    Condition,
    CreateCalendarEventAction,
    CreateNotificationAction,
    LogicalOperator,
    RuleAction,
    RuleEvent,
    RuleItem,
    SetFieldAction,
} from "@/types/rule";
import type { TaskPriority, TaskStatus } from "@/types/task";
import { useNavigate } from "react-router-dom";

interface RuleFormProps {
    rule?: RuleItem;
    onSuccess?: () => void;
}

const RULE_EVENT_LABELS: Record<RuleEvent, string> = {
    taskCreated: "Задача создана",
    taskUpdated: "Задача обновлена",
    taskDeleted: "Задача удалена",
    taskCompleted: "Задача завершена",
    taskOverdue: "Задача просрочена",
};

const SET_FIELD_ALLOWED_EVENTS: RuleEvent[] = ["taskCreated", "taskUpdated"];

const FIELD_OPTIONS = ["priority", "status", "dueDate", "title", "description", "projectName"];
const FIELD_LABELS: Record<string, string> = {
    priority: "Приоритет",
    status: "Статус",
    dueDate: "Дедлайн",
    title: "Заголовок",
    description: "Описание",
    projectName: "Проект",
};

const STATUS_OPTIONS: { value: TaskStatus; label: string }[] = [
    { value: "new", label: "Новая" },
    { value: "inProgress", label: "В работе" },
    { value: "done", label: "Готово" },
    { value: "canceled", label: "Отменена" },
];

const PRIORITY_OPTIONS: { value: TaskPriority; label: string }[] = [
    { value: "low", label: "Низкий" },
    { value: "medium", label: "Средний" },
    { value: "high", label: "Высокий" },
];

const OPERATOR_LABELS: Record<ComparisonOperator, string> = {
    eq: "=",
    neq: "≠",
    gt: ">",
    lt: "<",
};

const CONDITION_FIELD_OPTIONS = ["priority", "status", "projectName", "dueDate", "title"];
const CONDITION_FIELD_LABELS: Record<string, string> = {
    priority: "Приоритет",
    status: "Статус",
    projectName: "Проект",
    dueDate: "Дедлайн",
    title: "Заголовок",
};

// ──────────────────────────────────────────────
// Internal action shape with local id for keys
// ──────────────────────────────────────────────
type LocalAction =
    | ({ _id: number } & SetFieldAction)
    | ({ _id: number } & CreateNotificationAction)
    | ({ _id: number } & CreateCalendarEventAction);

let _actionIdCounter = 0;

const makeSetField = (): LocalAction => ({
    _id: ++_actionIdCounter,
    type: "SET_FIELD",
    field: "priority",
    value: "low",
});
const makeNotification = (): LocalAction => ({
    _id: ++_actionIdCounter,
    type: "CREATE_NOTIFICATION",
    description: "",
    offsetMinutes: 0,
});
const makeCalendar = (): LocalAction => ({
    _id: ++_actionIdCounter,
    type: "CREATE_CALENDAR_EVENT",
    durationMinutes: 60,
    offsetMinutes: 0,
    title: "",
    location: "",
    externalAccountId: "",
});

// ──────────────────────────────────────────────
// Smart value input — enum fields get selects, dueDate gets datetime, rest free text
// ──────────────────────────────────────────────
function FieldValueInput({
                             field,
                             value,
                             onChange,
                         }: {
    field: string;
    value: string;
    onChange: (v: string) => void;
}) {
    if (field === "priority") {
        return (
            <select
                className="w-full border rounded-md px-2 py-1.5 text-sm bg-background"
                value={value}
                onChange={(e) => onChange(e.target.value)}
            >
                {PRIORITY_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value}>{o.label}</option>
                ))}
            </select>
        );
    }
    if (field === "status") {
        return (
            <select
                className="w-full border rounded-md px-2 py-1.5 text-sm bg-background"
                value={value}
                onChange={(e) => onChange(e.target.value)}
            >
                {STATUS_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value}>{o.label}</option>
                ))}
            </select>
        );
    }
    if (field === "dueDate") {
        return (
            <Input
                type="datetime-local"
                value={value}
                onChange={(e) => onChange(e.target.value)}
            />
        );
    }
    return (
        <Input
            placeholder="Значение"
            value={value}
            onChange={(e) => onChange(e.target.value)}
        />
    );
}

// ──────────────────────────────────────────────
// Single action editor block
// ──────────────────────────────────────────────
const ACTION_TYPE_LABELS: Record<ActionType, string> = {
    SET_FIELD: "Изменить поле",
    CREATE_NOTIFICATION: "Уведомление",
    CREATE_CALENDAR_EVENT: "Событие в календаре",
};

function ActionEditor({
                          action,
                          canSetField,
                          onChange,
                          onRemove,
                      }: {
    action: LocalAction;
    canSetField: boolean;
    onChange: (updated: LocalAction) => void;
    onRemove: () => void;
}) {
    const availableTypes: ActionType[] = canSetField
        ? ["SET_FIELD", "CREATE_NOTIFICATION", "CREATE_CALENDAR_EVENT"]
        : ["CREATE_NOTIFICATION", "CREATE_CALENDAR_EVENT"];

    const handleTypeChange = (type: ActionType) => {
        if (type === "SET_FIELD") onChange({ ...makeSetField(), _id: action._id });
        else if (type === "CREATE_NOTIFICATION") onChange({ ...makeNotification(), _id: action._id });
        else onChange({ ...makeCalendar(), _id: action._id });
    };

    return (
        <div className="border rounded-xl p-3 bg-muted/30 space-y-3 relative">
            <button
                type="button"
                onClick={onRemove}
                className="absolute top-2.5 right-2.5 w-6 h-6 flex items-center justify-center text-muted-foreground hover:text-red-500 hover:bg-red-50 rounded-md transition-colors text-base leading-none border border-transparent hover:border-red-300"
            >
                ×
            </button>

            <div className="space-y-1.5 pr-8">
                <Label className="text-xs">Тип действия</Label>
                <select
                    className="w-full border rounded-md px-2 py-1.5 text-sm bg-background"
                    value={action.type}
                    onChange={(e) => handleTypeChange(e.target.value as ActionType)}
                >
                    {availableTypes.map((t) => (
                        <option key={t} value={t}>{ACTION_TYPE_LABELS[t]}</option>
                    ))}
                </select>
            </div>

            {action.type === "SET_FIELD" && (
                <div className="grid grid-cols-2 gap-3">
                    <div className="space-y-1.5">
                        <Label className="text-xs">Поле</Label>
                        <select
                            className="w-full border rounded-md px-2 py-1.5 text-sm bg-background"
                            value={action.field}
                            onChange={(e) => {
                                const newField = e.target.value;
                                const defaultValue =
                                    newField === "priority" ? "low"
                                        : newField === "status" ? "new"
                                            : "";
                                onChange({ ...action, field: newField, value: defaultValue } as LocalAction);
                            }}
                        >
                            {FIELD_OPTIONS.map((f) => (
                                <option key={f} value={f}>{FIELD_LABELS[f]}</option>
                            ))}
                        </select>
                    </div>
                    <div className="space-y-1.5">
                        <Label className="text-xs">Значение</Label>
                        <FieldValueInput
                            field={action.field}
                            value={action.value}
                            onChange={(v) => onChange({ ...action, value: v } as LocalAction)}
                        />
                    </div>
                </div>
            )}

            {action.type === "CREATE_NOTIFICATION" && (
                <>
                    <div className="space-y-1.5">
                        <Label className="text-xs">Текст уведомления</Label>
                        <Textarea
                            placeholder="Например: Задача просрочена, обратите внимание..."
                            value={action.description}
                            onChange={(e) => onChange({ ...action, description: e.target.value } as LocalAction)}
                        />
                    </div>
                    <div className="space-y-1.5">
                        <Label className="text-xs">Смещение (минуты)</Label>
                        <Input
                            type="number"
                            min={0}
                            placeholder="0"
                            value={action.offsetMinutes}
                            onChange={(e) => onChange({ ...action, offsetMinutes: Number(e.target.value) } as LocalAction)}
                        />
                    </div>
                </>
            )}

            {action.type === "CREATE_CALENDAR_EVENT" && (
                <div className="space-y-3">
                    <div className="space-y-1.5">
                        <Label className="text-xs">Название события</Label>
                        <Input
                            placeholder="Например: Встреча"
                            value={action.title}
                            onChange={(e) =>
                                onChange({ ...action, title: e.target.value } as LocalAction)
                            }
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-3">
                        <div className="space-y-1.5">
                            <Label className="text-xs">Длительность (мин)</Label>
                            <Input
                                type="number"
                                min={1}
                                value={action.durationMinutes}
                                onChange={(e) =>
                                    onChange({
                                        ...action,
                                        durationMinutes: Number(e.target.value),
                                    } as LocalAction)
                                }
                            />
                        </div>

                        <div className="space-y-1.5">
                            <Label className="text-xs">Смещение (мин)</Label>
                            <Input
                                type="number"
                                min={0}
                                value={action.offsetMinutes ?? 0}
                                onChange={(e) =>
                                    onChange({
                                        ...action,
                                        offsetMinutes: Number(e.target.value),
                                    } as LocalAction)
                                }
                            />
                        </div>
                    </div>

                    <div className="space-y-1.5">
                        <Label className="text-xs">Место</Label>
                        <Input
                            placeholder="Офис / Zoom"
                            value={action.location}
                            onChange={(e) =>
                                onChange({ ...action, location: e.target.value } as LocalAction)
                            }
                        />
                    </div>

                    <div className="space-y-1.5">
                        <Label className="text-xs">ID календаря (опционально)</Label>
                        <Input
                            placeholder="externalAccountId"
                            value={action.externalAccountId ?? ""}
                            onChange={(e) =>
                                onChange({
                                    ...action,
                                    externalAccountId: e.target.value,
                                } as LocalAction)
                            }
                        />
                    </div>
                </div>
            )}
        </div>
    );
}

// ──────────────────────────────────────────────
// Main form
// ──────────────────────────────────────────────
export const RuleForm = ({ rule, onSuccess }: RuleFormProps) => {
    const navigate = useNavigate();

    const [ruleEvent, setRuleEvent] = useState<RuleEvent>(rule?.ruleEvent ?? "taskCreated");
    const [logicalOp, setLogicalOp] = useState<LogicalOperator>(rule?.condition?.operator ?? "and");
    const [conditions, setConditions] = useState<Condition[]>(
        rule?.condition?.conditions ?? []
    );
    const [actions, setActions] = useState<LocalAction[]>(() => {
        if (rule?.action?.length) {
            return rule.action.map((a) => ({
                ...a,
                _id: ++_actionIdCounter,
            })) as LocalAction[];
        }
        return [makeSetField()];
    });
    const [isActive, setIsActive] = useState(rule?.isActive ?? true);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const canSetField = SET_FIELD_ALLOWED_EVENTS.includes(ruleEvent);

    const handleEventChange = (event: RuleEvent) => {
        setRuleEvent(event);
        if (!SET_FIELD_ALLOWED_EVENTS.includes(event)) {
            setActions((prev) =>
                prev.map((a) =>
                    a.type === "SET_FIELD" ? { ...makeNotification(), _id: a._id } : a
                )
            );
        }
    };

    const addCondition = () =>
        setConditions((prev) => [...prev, { field: "priority", operator: "eq", value: "low" }]);

    const removeCondition = (index: number) =>
        setConditions((prev) => prev.filter((_, i) => i !== index));

    const updateCondition = (index: number, key: keyof Condition, value: string) =>
        setConditions((prev) =>
            prev.map((c, i) => (i === index ? { ...c, [key]: value } : c))
        );

    const addAction = () => setActions((prev) => [...prev, makeNotification()]);
    const updateAction = (id: number, updated: LocalAction) =>
        setActions((prev) => prev.map((a) => (a._id === id ? updated : a)));
    const removeAction = (id: number) =>
        setActions((prev) => prev.filter((a) => a._id !== id));

    const validate = (): string | null => {
        if (conditions.some((c) => !c.value.trim()))
            return "Заполните значение для всех условий";
        if (actions.length === 0)
            return "Добавьте хотя бы одно действие";
        for (const a of actions) {
            if (a.type === "SET_FIELD" && !a.value.trim())
                return "Заполните значение для действия «Изменить поле»";
            if (a.type === "CREATE_NOTIFICATION" && !a.description.trim())
                return "Заполните текст уведомления";
            if (a.type === "CREATE_CALENDAR_EVENT" && !a.durationMinutes)
                return "Укажите длительность события в календаре";
        }
        return null;
    };

    const handleSubmit = async () => {
        const validationError = validate();
        if (validationError) {
            setError(validationError);
            return;
        }

        setError("");
        setLoading(true);

        // eslint-disable-next-line @typescript-eslint/no-unused-vars
        const actionPayload: RuleAction[] = actions.map(({ _id, ...rest }) => rest);

        const data: RuleCreateDto = {
            ruleEvent,
            conditions: { operator: logicalOp, conditions },
            actions: actionPayload,
        };

        try {
            if (rule) {
                await ruleApi.updateRule(rule.ruleId, data);
            } else {
                await ruleApi.createRule(data);
            }
            onSuccess?.();
            navigate(-1);
        } catch {
            setError("Ошибка сохранения правила");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto p-2 rounded-2xl space-y-6 pb-16">
            <h2 className="text-lg font-bold">
                {rule ? "Редактирование правила" : "Создание правила автоматизации"}
            </h2>

            {error && <div className="text-sm text-red-500">{error}</div>}

            {/* Event */}
            <div className="space-y-2">
                <Label>Событие-триггер</Label>
                <select
                    className="w-full border rounded-lg px-3 py-2 text-sm bg-background"
                    value={ruleEvent}
                    onChange={(e) => handleEventChange(e.target.value as RuleEvent)}
                >
                    {(Object.keys(RULE_EVENT_LABELS) as RuleEvent[]).map((key) => (
                        <option key={key} value={key}>{RULE_EVENT_LABELS[key]}</option>
                    ))}
                </select>
            </div>

            {/* Conditions */}
            <div className="space-y-2">
                <Label>Условия</Label>
                <div className="border rounded-xl p-3 space-y-3 bg-muted/30">
                    <div className="flex gap-2">
                        {(["and", "or"] as LogicalOperator[]).map((op) => (
                            <button
                                key={op}
                                type="button"
                                onClick={() => setLogicalOp(op)}
                                className={`px-3 py-1 text-xs rounded-md border transition-colors ${
                                    logicalOp === op
                                        ? "bg-primary text-primary-foreground border-primary"
                                        : "bg-background border-border text-muted-foreground hover:bg-muted"
                                }`}
                            >
                                {op}
                            </button>
                        ))}
                    </div>

                    {conditions.length === 0 && (
                        <p className="text-xs text-muted-foreground text-center py-2">
                            Нет условий — правило применяется ко всем событиям
                        </p>
                    )}

                    {conditions.map((cond, i) => (
                        <div key={i} className="grid grid-cols-[1fr_80px_1fr_28px] gap-2 items-center">
                            <select
                                className="border rounded-md px-2 py-1.5 text-sm bg-background"
                                value={cond.field}
                                onChange={(e) => {
                                    const newField = e.target.value;
                                    const defaultValue =
                                        newField === "priority" ? "low"
                                            : newField === "status" ? "new"
                                                : "";
                                    updateCondition(i, "field", newField);
                                    updateCondition(i, "value", defaultValue);
                                }}
                            >
                                {CONDITION_FIELD_OPTIONS.map((f) => (
                                    <option key={f} value={f}>{CONDITION_FIELD_LABELS[f]}</option>
                                ))}
                            </select>
                            <select
                                className="border rounded-md px-2 py-1.5 text-sm bg-background"
                                value={cond.operator}
                                onChange={(e) => updateCondition(i, "operator", e.target.value as ComparisonOperator)}
                            >
                                {(Object.keys(OPERATOR_LABELS) as ComparisonOperator[]).map((op) => (
                                    <option key={op} value={op}>{OPERATOR_LABELS[op]}</option>
                                ))}
                            </select>
                            <FieldValueInput
                                field={cond.field}
                                value={cond.value}
                                onChange={(v) => updateCondition(i, "value", v)}
                            />
                            <button
                                type="button"
                                onClick={() => removeCondition(i)}
                                className="w-7 h-7 flex items-center justify-center border rounded-md text-muted-foreground hover:text-red-500 hover:border-red-300 hover:bg-red-50 transition-colors text-base leading-none"
                            >
                                ×
                            </button>
                        </div>
                    ))}

                    <button
                        type="button"
                        onClick={addCondition}
                        className="w-full text-sm text-muted-foreground border border-dashed rounded-md py-1.5 hover:bg-muted transition-colors"
                    >
                        + Добавить условие
                    </button>
                </div>
            </div>

            {/* Actions */}
            <div className="space-y-2">
                <Label>Действия</Label>
                <div className="space-y-3">
                    {actions.map((action) => (
                        <ActionEditor
                            key={action._id}
                            action={action}
                            canSetField={canSetField}
                            onChange={(updated) => updateAction(action._id, updated)}
                            onRemove={() => removeAction(action._id)}
                        />
                    ))}
                    <button
                        type="button"
                        onClick={addAction}
                        className="w-full text-sm text-muted-foreground border border-dashed rounded-md py-1.5 hover:bg-muted transition-colors"
                    >
                        + Добавить действие
                    </button>
                </div>
            </div>

            {/* Status */}
            <div className="space-y-2">
                <Label>Статус правила</Label>
                <Tabs value={isActive ? "active" : "inactive"} onValueChange={(v) => setIsActive(v === "active")}>
                    <TabsList className="grid grid-cols-2 w-full">
                        <TabsTrigger value="active">Активно</TabsTrigger>
                        <TabsTrigger value="inactive">Неактивно</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <Button onClick={handleSubmit} disabled={loading} className="w-full mt-2">
                {loading ? "Сохраняем..." : rule ? "Сохранить" : "Создать правило"}
            </Button>
        </div>
    );
};