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

function renderAction(action: RuleAction) {
    switch (action.type) {
        case "SET_FIELD": {
            const a = action as SetFieldAction;
            return `Изменить ${a.field} → ${a.value}`;
        }

        case "CREATE_NOTIFICATION": {
            const a = action as CreateNotificationAction;
            return `Уведомление: ${a.description}`;
        }

        case "CREATE_CALENDAR_EVENT": {
            const a = action as CreateCalendarEventAction;
            return `Событие (${a.durationMinutes} мин)`;
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
                            <SheetTitle className="text-base leading-snug">
                                {eventLabels[rule.ruleEvent]}
                            </SheetTitle>
                        </SheetHeader>

                        {/* CONTENT */}
                        <div className="flex-1 overflow-y-auto space-y-5">
                            {/* УСЛОВИЯ */}
                            <div>
                                <div className="font-medium text-sm mb-2">
                                    Условия
                                </div>

                                {rule.condition.conditions.length ? (
                                    <div className="space-y-2">
                                        {rule.condition.conditions.map((c, i) => (
                                            <div
                                                key={i}
                                                className="text-sm border rounded-lg px-3 py-2 bg-muted/40"
                                            >
                                                {c.field} {c.operator} {c.value}
                                            </div>
                                        ))}
                                        <div className="text-xs text-muted-foreground">
                                            Оператор: {rule.condition.operator}
                                        </div>
                                    </div>
                                ) : (
                                    <div className="text-xs text-muted-foreground">
                                        Без условий (всегда выполняется)
                                    </div>
                                )}
                            </div>

                            {/* ACTIONS */}
                            <div>
                                <div className="font-medium text-sm mb-2">
                                    Действия
                                </div>

                                <div className="space-y-2">
                                    {rule.action.map((a, i) => (
                                        <div
                                            key={i}
                                            className="text-sm border rounded-lg px-3 py-2 bg-white"
                                        >
                                            {renderAction(a)}
                                        </div>
                                    ))}
                                </div>
                            </div>

                            {/* FOOTER */}
                            <div className="pt-4 mt-4 border-t flex gap-2">
                                <Button
                                    variant="outline"
                                    className="flex-1"
                                    onClick={() => onEdit?.(rule.ruleId)}
                                >
                                    Редактировать
                                </Button>

                                <Button
                                    variant="destructive"
                                    className="flex-1"
                                    onClick={() => setConfirmOpen(true)}
                                >
                                    Удалить
                                </Button>
                            </div>
                        </div>
                    </div>
                </SheetContent>
            </Sheet>

            {/* CONFIRM DELETE */}
            <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>
                            Удалить правило?
                        </AlertDialogTitle>
                        <AlertDialogDescription>
                            Это действие нельзя отменить.
                        </AlertDialogDescription>
                    </AlertDialogHeader>

                    <div className="flex gap-2 mt-4">
                        <AlertDialogCancel className="flex-1">
                            Отмена
                        </AlertDialogCancel>

                        <AlertDialogAction
                            className="flex-1"
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