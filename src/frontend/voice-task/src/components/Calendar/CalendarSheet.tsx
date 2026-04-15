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
import { Badge } from "@/components/ui/badge";
import { type FC, useState } from "react";
import type { CalendarEvent } from "@/types/calendarEvent";

interface Props {
    event: CalendarEvent | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
    onEdit?: (eventId: string) => void;
    onDelete?: (eventId: string) => void;
    onOpenTask?: (taskId: string) => void;
}

const formatTime = (date: string) => {
    return new Date(date).toLocaleString();
};

export const CalendarEventSheet: FC<Props> = ({
                                                  event,
                                                  open,
                                                  onOpenChange,
                                                  onEdit,
                                                  onDelete,
                                                  onOpenTask,
                                              }) => {
    const [confirmOpen, setConfirmOpen] = useState(false);

    if (!event) return null;

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
                                {event.title}
                            </SheetTitle>

                            <div className="flex flex-wrap gap-2">
                                {event.externalAccountId && (
                                    <Badge variant="secondary">
                                        External
                                    </Badge>
                                )}
                            </div>
                        </SheetHeader>

                        {/* CONTENT */}
                        <div className="flex-1 overflow-y-auto space-y-5">
                            {/* time */}
                            <div className="space-y-3 text-sm">
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">
                                        Начало
                                    </span>
                                    <span className="font-medium">
                                        {formatTime(event.startTime)}
                                    </span>
                                </div>

                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">
                                        Конец
                                    </span>
                                    <span className="font-medium">
                                        {formatTime(event.endTime)}
                                    </span>
                                </div>

                                {event.location && (
                                    <div className="flex justify-between">
                                        <span className="text-muted-foreground">
                                            Локация
                                        </span>
                                        <span className="font-medium">
                                            {event.location}
                                        </span>
                                    </div>
                                )}
                            </div>

                            {/* linked task */}
                            {event.taskId && (
                                <div>
                                    <div className="font-medium text-sm mb-2">
                                        Связанная задача
                                    </div>

                                    <div
                                        className="
                                            p-3 rounded-xl border bg-muted/40
                                            cursor-pointer active:scale-[0.98] transition
                                        "
                                        onClick={() =>
                                            onOpenTask?.(event.taskId!)
                                        }
                                    >
                                        <div className="text-sm" >
                                            Открыть задачу
                                        </div>
                                    </div>
                                </div>
                            )}

                            {/* divider */}
                            <div className="h-px bg-border" />

                            {/* actions */}
                            <div className="pt-4 flex gap-2">
                                <Button
                                    variant="outline"
                                    className="flex-1"
                                    onClick={() =>
                                        onEdit?.(event.eventId)
                                    }
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

            {/* confirm delete */}
            <AlertDialog
                open={confirmOpen}
                onOpenChange={setConfirmOpen}
            >
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>
                            Удалить событие?
                        </AlertDialogTitle>
                        <AlertDialogDescription>
                            Это действие нельзя отменить. Событие будет удалено.
                        </AlertDialogDescription>
                    </AlertDialogHeader>

                    <div className="flex gap-2 mt-4">
                        <AlertDialogCancel className="flex-1">
                            Отмена
                        </AlertDialogCancel>

                        <AlertDialogAction
                            className="flex-1"
                            onClick={() =>
                                onDelete?.(event.eventId)
                            }
                        >
                            Удалить
                        </AlertDialogAction>
                    </div>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
};