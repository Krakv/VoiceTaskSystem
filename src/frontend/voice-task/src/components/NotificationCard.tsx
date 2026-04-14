import { Badge } from "@/components/ui/badge";
import { notificationStatusMap, serviceIconMap } from "@/utils/notification.utils";
import type { FC } from "react";
import type { NotificationItem } from "@/types/notification";
import {CheckSquareIcon} from "lucide-react";
import {formatSmartDate} from "@/utils/task.utils.ts";

interface Props {
    notification: NotificationItem;
    onOpen: (n: NotificationItem) => void;
}

export const NotificationCard: FC<Props> = ({ notification, onOpen }) => {
    const status = notificationStatusMap[notification.status];
    const service = serviceIconMap[notification.serviceId];
    const isPending = notification.status === "pending";

    return (
        <div
            onClick={() => onOpen(notification)}
            className={`p-4 rounded-2xl border active:scale-[0.98] transition space-y-2 bg-white ${
                !isPending ? "opacity-50" : ""
            }`}
        >
            <div className="flex gap-3 items-start">
                {service && (
                    <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5 ${service.bgClassName}`}>
                        <service.Icon className={`w-4 h-4 ${service.colorClassName}`} />
                    </div>
                )}
                <div className="flex-1 min-w-0 space-y-1">
                    <div className="text-sm font-medium leading-snug truncate">
                        {notification.description}
                    </div>
                    <div className="flex flex-wrap gap-2 items-center">
                        <span className="text-xs text-muted-foreground">
                            {formatSmartDate(notification.scheduledAt)}
                        </span>
                        {status && (
                            <Badge className={status.className}>{status.label}</Badge>
                        )}
                        {notification.task && (
                            <span className="text-xs text-muted-foreground flex items-center gap-1">
                                <CheckSquareIcon className="w-3 h-3 opacity-60" />
                                {notification.task.title}
                            </span>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};