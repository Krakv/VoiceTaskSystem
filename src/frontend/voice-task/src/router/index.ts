import Login from '@/pages/Login'
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskForm} from "@/components/TaskForm.tsx";

export const privateRoutes = [
    { path: '/tasks', component: MainPage },
    { path: '/create', component: TaskForm },
];

export const publicRoutes = [
    { path: '*', component: Login },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];