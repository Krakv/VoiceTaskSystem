import Login from '@/pages/Login'
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskEditPage} from "@/pages/TaskEditPage.tsx";
import {TaskCreatePage} from "@/pages/TaskCreatePage.tsx";
import {VoiceInputPage} from "@/pages/VoiceInputPage.tsx";
import {VoiceResultPage} from "@/pages/VoiceResultPage.tsx";
import {CommandRequestEditPage} from "@/pages/CommandRequestEditPage.tsx";

export const privateRoutes = [
    { path: '/tasks', component: MainPage },
    { path: '/create', component: TaskCreatePage },
    { path: '/create/voice', component: VoiceInputPage },
    { path: '/create/voice/:id', component: VoiceResultPage },
    { path: '/create/voice/:id/edit', component: CommandRequestEditPage },
    { path: '/tasks/:taskId/edit', component: TaskEditPage },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];