import {
    Sheet,
    SheetContent,
    SheetHeader,
    SheetTitle,
} from "@/components/ui/sheet";
import { Badge } from "@/components/ui/badge";
import type { NotificationItem } from "@/types/notification";
import { type FC, useState } from "react";
import { notificationStatusMap, serviceIconMap } from "@/utils/notification.utils";
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
import {formatSmartDate} from "@/utils/task.utils.ts";

interface Props {
    notification: NotificationItem | null;
    open: boolean;
    onOpenChange: (v: boolean) => void;
    onEdit?: (notificationId: string) => void;
    onCancel?: (notificationId: string) => void;
}

export const NotificationSheet: FC<Props> = ({ notification, open, onOpenChange, onEdit, onCancel }) => {
    const [confirmOpen, setConfirmOpen] = useState(false);
    if (!notification) return null;

    const status = notificationStatusMap[notification.status];
    const service = serviceIconMap[notification.serviceId];
    const isPending = notification.status === "pending";
    const isOverdue = isPending && new Date(notification.scheduledAt) < new Date();

    return (
        <>
            <Sheet open={open} onOpenChange={onOpenChange}>
                <SheetContent
                    side="bottom"
                    className="rounded-t-2xl h-auto flex flex-col p-0 w-full max-w-md mx-auto"
                >
                    <div className="px-4 pt-4 pb-6 flex flex-col gap-5">
                        <SheetHeader className="text-left space-y-2">
                            <div className="flex items-center gap-3">
                                {service && (
                                    <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${service.bgClassName}`}>
                                        <service.Icon className={`w-4 h-4 ${service.colorClassName}`} />
                                    </div>
                                )}
                                <SheetTitle className="text-base leading-snug">
                                    {service?.label ?? "Уведомление"}
                                </SheetTitle>
                            </div>
                            {status && (
                                <div>
                                    <Badge className={status.className}>{status.label}</Badge>
                                </div>
                            )}
                        </SheetHeader>

                        <p className="text-sm text-muted-foreground leading-relaxed">
                            {notification.description}
                        </p>

                        <div className="space-y-3 text-sm">
                            <div className="flex justify-between">
                                <span className="text-muted-foreground">Запланировано</span>
                                <span className={isOverdue ? "font-medium text-red-500" : "font-medium"}>
                                    {formatSmartDate(notification.scheduledAt)}
                                </span>
                            </div>

                            {notification.sentAt && (
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Отправлено</span>
                                    <span className="font-medium">
                                        {formatSmartDate(notification.sentAt)}
                                    </span>
                                </div>
                            )}

                            {notification.task && (
                                <div className="flex justify-between">
                                    <span className="text-muted-foreground">Задача</span>
                                    <span className="font-medium">{notification.task.title}</span>
                                </div>
                            )}
                        </div>

                        <div className="pt-2 border-t flex gap-2">
                            {isPending && (
                                <>
                                    <Button
                                        variant="outline"
                                        className="flex-1"
                                        onClick={() => onEdit?.(notification.notificationId)}
                                    >
                                        Редактировать
                                    </Button>
                                    <Button
                                        variant="destructive"
                                        className="flex-1"
                                        onClick={() => setConfirmOpen(true)}
                                    >
                                        Отменить
                                    </Button>
                                </>
                            )}
                        </div>
                    </div>
                </SheetContent>
            </Sheet>

            <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
                <AlertDialogContent>
                    <AlertDialogHeader>
                        <AlertDialogTitle>Отменить уведомление?</AlertDialogTitle>
                        <AlertDialogDescription>
                            Уведомление не будет отправлено.
                        </AlertDialogDescription>
                    </AlertDialogHeader>
                    <div className="flex gap-2 mt-4">
                        <AlertDialogCancel className="flex-1">Назад</AlertDialogCancel>
                        <AlertDialogAction
                            className="flex-1"
                            onClick={() => onCancel?.(notification.notificationId)}
                        >
                            Отменить
                        </AlertDialogAction>
                    </div>
                </AlertDialogContent>
            </AlertDialog>
        </>
    );
};