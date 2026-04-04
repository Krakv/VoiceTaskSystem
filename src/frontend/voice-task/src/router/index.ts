import Login from '@/pages/Login'
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskCreateForm} from "@/components/TaskCreateForm.tsx";

export const privateRoutes = [
    { path: '/tasks', component: MainPage },
    { path: '/create', component: TaskCreateForm },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];