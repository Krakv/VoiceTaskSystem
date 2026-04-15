import {Send, Mail, type LucideProps} from "lucide-react";
import type {NotificationServiceType, NotificationStatus} from "@/types/notification";
import type {FC} from "react";

export const notificationStatusMap: Record<NotificationStatus, { label: string; className: string }> = {
    pending:    { label: "Ожидает",    className: "bg-amber-50  text-amber-800  border-amber-200" },
    processing: { label: "В процессе", className: "bg-blue-50   text-blue-800   border-blue-200" },
    sent:       { label: "Отправлено", className: "bg-green-50  text-green-800  border-green-200" },
    failed:     { label: "Ошибка",     className: "bg-red-50    text-red-800    border-red-200" },
    cancelled:  { label: "Отменено",   className: "bg-gray-100  text-gray-600   border-gray-200" },
};

export const serviceIconMap: Record<NotificationServiceType, { Icon: FC<LucideProps>; bgClassName: string; colorClassName: string, label: string }> = {
    "telegram": { Icon: Send,  bgClassName: "bg-blue-50",   colorClassName: "text-blue-700", label: "Telegram" },   // Telegram
    "email": { Icon: Mail,  bgClassName: "bg-pink-50",   colorClassName: "text-pink-700", label: "Email" },   // Email
};
