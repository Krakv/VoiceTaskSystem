import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { notificationApi } from "@/api/notification.api";
import { NotificationForm } from "@/components/NotificationForm";
import type { NotificationItem } from "@/types/notification";

export const NotificationEditPage = () => {
    const { notificationId } = useParams<{ notificationId: string }>();
    const [notification, setNotification] = useState<NotificationItem | undefined>(undefined);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!notificationId) return;
        notificationApi.getNotificationById(notificationId)
            .then((res) => {
                setNotification(res.data.data);
            })
            .finally(() => setLoading(false));
    }, [notificationId]);


    return (
        <>
            {loading ? (<div>Загрузка...</div>) : <NotificationForm notification={notification} /> }
        </>
    );
};