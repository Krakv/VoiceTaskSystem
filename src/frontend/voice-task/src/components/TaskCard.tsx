import { Checkbox } from "@/components/ui/checkbox";
import { Badge } from "@/components/ui/badge";
import { formatSmartDate, statusMap, priorityMap } from "@/utils/task.utils";
import type {FC} from "react";
import type {Task} from "@/types/task.ts";


interface Props {
    task: Task;
    onOpen: (task: Task) => void;
    onToggle: (task: Task) => void;
}

export const TaskCard: FC<Props> = ({ task, onOpen, onToggle }) => {
    const status = statusMap[task.status];
    const priority = priorityMap[task.priority];

    return (
        <div
            onClick={() => onOpen(task)}
            className="
        p-4 rounded-2xl border bg-white
        active:scale-[0.98] transition
        space-y-2
      "
        >
            <div className="flex gap-3">
                <div
                    onClick={(e) => {
                        e.stopPropagation();
                        onToggle(task);
                    }}
                >
                    <Checkbox checked={task.status === "done"} />
                </div>

                <div className="flex-1 space-y-1">
                    <div
                        className={`
              text-sm font-medium leading-snug
              ${task.status === "done" && "line-through opacity-50"}
            `}
                    >
                        {task.title}
                    </div>

                    <div className="flex flex-wrap gap-2 items-center">
                        {task.dueDate && (
                            <span className="text-xs text-muted-foreground">
                {formatSmartDate(task.dueDate)}
              </span>
                        )}

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

                        {task.projectName && (
                            <span className="text-xs text-muted-foreground">
                {task.projectName}
              </span>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};