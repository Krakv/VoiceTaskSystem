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
import {RuleCreatePage} from "@/pages/Rule/RuleCreatePage.tsx";
import {RulesPage} from "@/pages/Rule/RulesPage.tsx";
import {RuleEditPage} from "@/pages/Rule/RuleEditPage.tsx";
import {CalendarPage} from "@/pages/Calendar/CalendarPage.tsx";
import {CalendarCreatePage} from "@/pages/Calendar/CalendarCreatePage.tsx";
import {CalendarEditPage} from "@/pages/Calendar/CalendarEditPage.tsx";

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
    { path: '/rules', component: RulesPage },
    { path: '/rules/create', component: RuleCreatePage },
    { path: '/rules/:ruleId/edit', component: RuleEditPage },
    { path: '/calendar', component: CalendarPage },
    { path: '/calendar/create', component: CalendarCreatePage },
    { path: '/calendar/:eventId/edit', component: CalendarEditPage },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];