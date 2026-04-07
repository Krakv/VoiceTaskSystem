import {
    Sheet,
    SheetContent,
    SheetHeader,
    SheetTitle,
    SheetDescription,
} from "@/components/ui/sheet";
import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import type { Task } from "@/types/task";
import {type FC, useState} from "react";
import { formatSmartDate, statusMap, priorityMap } from "@/utils/task.utils";
import {Button} from "@/components/ui/button.tsx";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent, AlertDialogDescription,
    AlertDialogHeader,
    AlertDialogTitle
} from "@/components/ui/alert-dialog.tsx";

interface Props {
    task: Task | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
    onOpenTask?: (taskId: string) => void;
    onToggleSubtask?: (taskId: string, status: string) => void;
    onEdit?: (taskId: string) => void;
    onDelete?: (taskId: string) => void;
}

export const TaskSheet: FC<Props> = ({task,open,onOpenChange,onOpenTask,onToggleSubtask,onEdit,onDelete}) => {
    const [confirmOpen, setConfirmOpen] = useState(false);
    if (!task) return null;
    const isOverdue = task.dueDate && new Date(task.dueDate) < new Date();

    const status =
        statusMap[task.status as keyof typeof statusMap] ?? statusMap.new;

    const priority =
        priorityMap[task.priority as keyof typeof priorityMap];

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
                            {task.title}
                        </SheetTitle>

                        <div className="flex flex-wrap gap-2">
                            {priority && (
                                <Badge className={priority.className}>
                                    {priority.label}
                                </Badge>
                            )}

                            {status && (
                                <Badge className={status.className}>
                                    {status.label}
                                </Badge>
                            )}
                        </div>
                    </SheetHeader>

                    {/* SCROLLABLE CONTENT */}
                    <div className="flex-1 overflow-y-auto space-y-5">
                        {/* описание */}
                        {task.description && (
                            <SheetDescription asChild>
                                <p className="text-sm text-muted-foreground leading-relaxed">
                                    {task.description}
                                </p>
                            </SheetDescription>
                        )}

                        {/* мета */}
                        <div className="space-y-3 text-sm">
                            {task.projectName && (
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Проект</span>
                                    <span className="font-medium">{task.projectName}</span>
                                </div>
                            )}

                            {task.dueDate && (
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Срок</span>
                                    <span className={isOverdue ? "font-medium text-red-500" : "font-medium"}>
                                      {formatSmartDate(task.dueDate)}
                                    </span>
                                </div>
                            )}
                        </div>

                        {task.parentTask && (
                            <div>
                                <div className="font-medium text-sm mb-2">
                                    Родительская задача
                                </div>

                                <div
                                    className="
                                        flex items-center gap-3
                                        p-3 rounded-xl border bg-muted/40
                                        active:scale-[0.98] transition cursor-pointer
                                      "
                                    onClick={() => onOpenTask?.(task.parentTask!.taskId)}
                                >
                                    {/* чекбокс */}
                                    <Checkbox checked={task.parentTask.status === "done"} />

                                    <div className="flex-1">
                                        <div
                                            className={`text-sm ${
                                                task.parentTask.status === "done" &&
                                                "line-through opacity-50"
                                            }`}
                                        >
                                            {task.parentTask.title}
                                        </div>

                                        <div className="flex gap-2 mt-1">
                                            {/* приоритет */}
                                            {task.parentTask.priority && (
                                                <Badge
                                                    className={
                                                        priorityMap[
                                                            task.parentTask.priority as keyof typeof priorityMap
                                                            ]?.className
                                                    }
                                                >
                                                    {
                                                        priorityMap[
                                                            task.parentTask.priority as keyof typeof priorityMap
                                                            ]?.label
                                                    }
                                                </Badge>
                                            )}

                                            {/* статус */}
                                            {task.parentTask.status && (
                                                <Badge
                                                    className={
                                                        statusMap[
                                                            task.parentTask.status as keyof typeof statusMap
                                                            ]?.className
                                                    }
                                                >
                                                    {
                                                        statusMap[
                                                            task.parentTask.status as keyof typeof statusMap
                                                            ]?.label
                                                    }
                                                </Badge>
                                            )}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        )}

                        {/* divider */}
                        <div className="h-px bg-border" />

                        {/* ПОДЗАДАЧИ */}
                        <div>
                            <div className="font-medium text-sm mb-3">
                                Подзадачи
                            </div>

                            <div className="space-y-2">
                                {task.childrenTasks?.length ? (
                                    task.childrenTasks.map((sub) => {
                                        const subStatus =
                                            statusMap[sub.status as keyof typeof statusMap];

                                        const subPriority =
                                            priorityMap[sub.priority as keyof typeof priorityMap];

                                        return (
                                            <div
                                                key={sub.taskId}
                                                className="
                                                  flex items-center gap-3
                                                  p-3 rounded-xl border bg-white
                                                  active:scale-[0.98] transition
                                                "
                                            >
                                                {/* toggle */}
                                                <Checkbox
                                                    checked={sub.status === "done"}
                                                    onCheckedChange={() =>
                                                        onToggleSubtask?.(sub.taskId, sub.status)
                                                    }
                                                />

                                                {/* content */}
                                                <div
                                                    className="flex-1 cursor-pointer"
                                                    onClick={() => onOpenTask?.(sub.taskId)}
                                                >
                                                    <div
                                                        className={`text-sm ${
                                                            sub.status === "done" &&
                                                            "line-through opacity-50"
                                                        }`}
                                                    >
                                                        {sub.title}
                                                    </div>

                                                    <div className="flex gap-2 mt-1">
                                                        {subPriority && (
                                                            <Badge className={subPriority.className}>
                                                                {subPriority.label}
                                                            </Badge>
                                                        )}

                                                        {subStatus && (
                                                            <Badge className={subStatus.className}>
                                                                {subStatus.label}
                                                            </Badge>
                                                        )}
                                                    </div>
                                                </div>
                                            </div>
                                        );
                                    })
                                ) : (
                                    <div className="text-xs text-muted-foreground">
                                        Нет подзадач
                                    </div>
                                )}
                            </div>
                        </div>

                        <div className="pt-4 mt-4 border-t flex gap-2">
                            <Button
                                variant="outline"
                                className="flex-1"
                                onClick={() => onEdit?.(task?.taskId)}
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
        <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>
                        Удалить задачу?
                    </AlertDialogTitle>
                    <AlertDialogDescription>
                        Это действие нельзя отменить. Задача будет удалена навсегда.
                    </AlertDialogDescription>
                </AlertDialogHeader>

                <div className="flex gap-2 mt-4">
                    <AlertDialogCancel className="flex-1">
                        Отмена
                    </AlertDialogCancel>

                    <AlertDialogAction
                        className="flex-1"
                        onClick={() => onDelete?.(task.taskId)}
                    >
                        Удалить
                    </AlertDialogAction>
                </div>
            </AlertDialogContent>
        </AlertDialog>
        </>
    );
};