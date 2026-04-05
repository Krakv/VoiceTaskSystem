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
import type { FC } from "react";
import { formatDate, statusMap, priorityMap } from "@/utils/task.utils";

interface Props {
    task: Task | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
}

export const TaskSheet: FC<Props> = ({task,open,onOpenChange,}) => {
    if (!task) return null;

    const status =
        statusMap[task.status as keyof typeof statusMap] ?? statusMap.new;

    const priority =
        priorityMap[task.priority as keyof typeof priorityMap];

    return (
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
                                    <span className="font-medium">
              {formatDate(task.dueDate)}
            </span>
                                </div>
                            )}
                        </div>

                        {/* divider */}
                        <div className="h-px bg-border" />

                        {/* ПОДЗАДАЧИ */}
                        <div>
                            <div className="font-medium text-sm mb-3">
                                Подзадачи
                            </div>

                            <div className="space-y-2">
                                {task.subtasks?.length ? (
                                    task.subtasks.map((sub) => (
                                        <div
                                            key={sub.taskId}
                                            className="flex items-center gap-3 p-3 rounded-xl border bg-white"
                                        >
                                            <Checkbox checked={sub.status === "done"} />

                                            <span
                                                className={`text-sm ${
                                                    sub.status === "done" &&
                                                    "line-through opacity-50"
                                                }`}
                                            >
                  {sub.title}
                </span>
                                        </div>
                                    ))
                                ) : (
                                    <div className="text-xs text-muted-foreground">
                                        Нет подзадач
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
            </SheetContent>
        </Sheet>
    );
};