import {
    Sheet,
    SheetContent,
    SheetHeader,
    SheetTitle
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { type FC, useState } from "react";
import type {
    RuleItem,
    RuleAction,
    SetFieldAction,
    CreateCalendarEventAction,
    CreateNotificationAction
} from "@/types/rule";

interface Props {
    rule: RuleItem | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
    onEdit?: (ruleId: string) => void;
    onDelete?: (ruleId: string) => void;
}

const eventLabels: Record<string, string> = {
    taskCreated: "Создание задачи",
    taskUpdated: "Обновление задачи",
    taskDeleted: "Удаление задачи",
    taskCompleted: "Завершение задачи",
    taskOverdue: "Просрочка задачи",
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

const operatorLabels: Record<string, string> = {
    eq: "=",
    neq: "≠",
    gt: ">",
    lt: "<",
    and: "все условия (И)",
    or: "любое условие (ИЛИ)",
};

function renderAction(action: RuleAction) {
    switch (action.type) {
        case "SET_FIELD": {
            const a = action as SetFieldAction;
            const field = fieldLabels[a.field] || a.field;
            const value = valueLabels[a.value] || a.value;
            return `Изменить ${field} → ${value}`;
        }

        case "CREATE_NOTIFICATION": {
            const a = action as CreateNotificationAction;
            const service = a.serviceId === "telegram" ? "ТГ" : "Email";
            return `Уведомление (${service}): ${a.description}`;
        }

        case "CREATE_CALENDAR_EVENT": {
            const a = action as CreateCalendarEventAction;
            return `Событие в календаре (${a.durationMinutes} мин)`;
        }

        default:
            return action.type;
    }
}

export const RuleSheet: FC<Props> = ({
                                         rule,
                                         open,
                                         onOpenChange,
                                         onEdit,
                                         onDelete,
                                     }) => {
    const [confirmOpen, setConfirmOpen] = useState(false);

    if (!rule) return null;

    return (
        <>
            <Sheet open={open} onOpenChange={onOpenChange}>
                <SheetContent
                    side="bottom"
                    className="
                        rounded-t-2xl h-[85vh] flex flex-col p-0
                        w-full max-w-md mx-auto
                    "
                >
                    <div className="px-4 pt-4 pb-6 flex flex-col h-full">
                        {/* HEADER */}
                        <SheetHeader className="text-left space-y-2 mb-4">
                            <SheetTitle className="text-lg font-bold">
                                {eventLabels[rule.ruleEvent]}
                            </SheetTitle>
                        </SheetHeader>

                        {/* CONTENT */}
                        <div className="flex-1 overflow-y-auto space-y-5">
                            {/* УСЛОВИЯ */}
                            <div>
                                <div className="font-semibold text-sm mb-3 text-muted-foreground uppercase tracking-wider">
                                    Условия выполнения
                                </div>

                                {rule.condition.conditions.length ? (
                                    <div className="space-y-2">
                                        <div className="text-xs font-medium text-blue-600 mb-2">
                                            Должны быть выполнены {operatorLabels[rule.condition.operator]}:
                                        </div>
                                        {rule.condition.conditions.map((c, i) => (
                                            <div
                                                key={i}
                                                className="text-sm border rounded-xl px-3 py-2.5 bg-muted/30 flex justify-between"
                                            >
                                                <span className="font-medium">{fieldLabels[c.field] || c.field}</span>
                                                <span className="text-muted-foreground">{operatorLabels[c.operator] || c.operator}</span>
                                                <span className="font-medium">{valueLabels[c.value] || c.value}</span>
                                            </div>
                                        ))}
                                    </div>
                                ) : (
                                    <div className="text-sm text-muted-foreground p-3 border border-dashed rounded-xl text-center">
                                        Всегда активно (без дополнительных условий)
                                    </div>
                                )}
                            </div>

                            {/* ACTIONS */}
                            <div>
                                <div className="font-semibold text-sm mb-3 text-muted-foreground uppercase tracking-wider">
                                    Автоматические действия
                                </div>

                                <div className="space-y-2">
                                    {rule.action.map((a, i) => (
                                        <div
                                            key={i}
                                            className="text-sm border rounded-xl px-4 py-3 bg-white shadow-sm flex items-center gap-3"
                                        >
                                            <div className="w-1.5 h-1.5 rounded-full bg-primary" />
                                            {renderAction(a)}
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>

                        {/* FOOTER */}
                        <div className="pt-4 border-t flex gap-3">
                            <Button
                                variant="outline"
                                className="flex-1 rounded-xl"
                                onClick={() => onEdit?.(rule.ruleId)}
                            >
                                Редактировать
                            </Button>

                            <Button
                                variant="destructive"
                                className="flex-1 rounded-xl"
                                onClick={() => setConfirmOpen(true)}
                            >
                                Удалить
                            </Button>
                        </div>
                    </div>
                </SheetContent>
            </Sheet>

            {/* CONFIRM DELETE */}
            <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
                <AlertDialogContent className="rounded-2xl max-w-[90vw]">
                    <AlertDialogHeader>
                        <AlertDialogTitle>
                            Удалить правило?
                        </AlertDialogTitle>
                        <AlertDialogDescription>
                            Это действие нельзя будет отменить. Правило автоматизации перестанет работать.
                        </AlertDialogDescription>
                    </AlertDialogHeader>

                    <div className="flex gap-3 mt-4">
                        <AlertDialogCancel className="flex-1 rounded-xl border-none bg-muted hover:bg-muted/80">
                            Отмена
                        </AlertDialogCancel>

                        <AlertDialogAction
                            className="flex-1 rounded-xl bg-destructive text-destructive-foreground hover:bg-destructive/90"
                            onClick={() => onDelete?.(rule.ruleId)}
                        >
                            Удалить
                        </AlertDialogAction>
                    </div>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
};