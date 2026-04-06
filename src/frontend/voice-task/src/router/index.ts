import Login from '@/pages/Login'
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskEditPage} from "@/pages/TaskEditPage.tsx";
import {TaskCreatePage} from "@/pages/TaskCreatePage.tsx";

export const privateRoutes = [
    { path: '/tasks', component: MainPage },
    { path: '/create', component: TaskCreatePage },
    { path: '/tasks/:taskId/edit', component: TaskEditPage },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];