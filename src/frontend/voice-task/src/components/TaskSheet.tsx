import {
    Sheet,
    SheetContent, SheetDescription,
    SheetHeader,
    SheetTitle,
} from "@/components/ui/sheet";
import { Checkbox } from "@/components/ui/checkbox";
import type { Task } from "@/types/task";
import type {FC} from "react";

interface Props {
    task: Task | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
}

export const TaskSheet: FC<Props> = ({ task, open, onOpenChange, }) => {
    if (!task) return null;

    return (
        <Sheet open={open} onOpenChange={onOpenChange}>
            <SheetContent side="bottom" className="rounded-t-2xl">
                <SheetHeader>
                    <SheetTitle>{task.title}</SheetTitle>
                </SheetHeader>

                {task.description && (
                    <SheetDescription>
                        <p className="text-sm text-muted-foreground">
                            {task.description}
                        </p>
                    </SheetDescription>
                )}

                <div className="mt-4 space-y-4">



                    <div className="text-sm space-y-1">
                        <div><b>Проект:</b> {task.projectName}</div>
                        <div><b>Статус:</b> {task.status}</div>
                        <div><b>Срок:</b> {task.dueDate}</div>
                    </div>

                    <div>
                        <div className="font-medium mb-2">Подзадачи</div>

                        <div className="space-y-2">
                            {task.subtasks?.map((sub) => (
                                <div key={sub.taskId} className="flex items-center gap-2">
                                    <Checkbox checked={sub.status === "done"} />
                                    <span className="text-sm">{sub.title}</span>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </SheetContent>
        </Sheet>
    );
};