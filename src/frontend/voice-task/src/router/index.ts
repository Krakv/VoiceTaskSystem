import Login from '@/pages/Login'
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskEditPage} from "@/pages/TaskEditPage.tsx";
import {TaskCreatePage} from "@/pages/TaskCreatePage.tsx";
import {VoiceInputPage} from "@/pages/VoiceInputPage.tsx";
import {VoiceResultPage} from "@/pages/VoiceResultPage.tsx";
import {CommandRequestEditPage} from "@/pages/CommandRequestEditPage.tsx";
import {NotificationsPage} from "@/pages/NotificationsPage.tsx";
import {NotificationEditPage} from "@/pages/NotificationEditPage.tsx";
import {NotificationCreatePage} from "@/pages/NotificationCreatePage.tsx";

export const privateRoutes = [
    { path: '/tasks', component: MainPage },
    { path: '/create', component: TaskCreatePage },
    { path: '/create/voice', component: VoiceInputPage },
    { path: '/create/voice/:id', component: VoiceResultPage },
    { path: '/create/voice/:id/edit', component: CommandRequestEditPage },
    { path: '/tasks/:taskId/edit', component: TaskEditPage },
    { path: '/notifications', component: NotificationsPage },
    { path: '/notifications/create', component: NotificationCreatePage },
    { path: '/notifications/:notificationId/edit', component: NotificationEditPage },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];