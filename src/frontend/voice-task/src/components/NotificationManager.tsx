import { useCallback, useEffect, useState } from "react";
import { notificationApi } from "@/api/notification.api";
import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import { Plus } from "lucide-react";
import type { NotificationItem } from "@/types/notification";
import { NotificationCard } from "@/components/NotificationCard";
import { NotificationSheet } from "@/components/NotificationSheet";
import { Skeleton } from "@/components/ui/skeleton";

export const NotificationManager = () => {
    const [notifications, setNotifications] = useState<NotificationItem[]>([]);
    const [selected, setSelected] = useState<NotificationItem | null>(null);
    const [open, setOpen] = useState(false);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    const fetchNotifications = async () => {
        setLoading(true);
        try {
            const res = await notificationApi.getNotifications();
            setNotifications(res.data.data);
        } finally {
            setLoading(false);
        }
    };

    const handleOpen = async (notificationId: string) => {
        const { data } = await notificationApi.getNotificationById(notificationId);
        setSelected(data.data);
        setOpen(true);
    };

    const handleEdit = useCallback((notificationId: string) => {
        navigate(`/notifications/${notificationId}/edit`);
    }, [navigate]);

    useEffect(() => {
        fetchNotifications();
    }, []);

    return (
        <div className="flex flex-col gap-2">

            {loading ? (
                <>
                    <Skeleton className="p-4 rounded-2xl border bg-gray-100 h-16 w-full" />
                    <Skeleton className="p-4 rounded-2xl border bg-gray-100 h-16 w-full" />
                    <Skeleton className="p-4 rounded-2xl border bg-gray-100 h-16 w-full" />
                </>
            ) : notifications.length === 0 ? (
                <div className="text-sm text-muted-foreground">Уведомлений нет</div>
            ) : (
                notifications.map((n) => (
                    <NotificationCard
                        key={n.notificationId}
                        notification={n}
                        onOpen={(n) => handleOpen(n.notificationId)}
                    />
                ))
            )}

            <NotificationSheet
                notification={selected}
                open={open}
                onOpenChange={setOpen}
                onEdit={handleEdit}
                onCancel={async (notificationId) => {
                    await notificationApi.deleteNotification(notificationId);
                    setOpen(false);
                    await fetchNotifications();
                }}
            />

            <div className="fixed bottom-16 right-4 flex max-w-md justify-end">
                <Button
                    onClick={() => navigate("/notifications/create")}
                    className="relative rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
                >
                    <Plus className="w-10 h-10" />
                </Button>
            </div>
        </div>
    );
};