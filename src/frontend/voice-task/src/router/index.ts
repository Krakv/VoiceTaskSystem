import Login from '@/pages/Login'
import {DefaultPage} from "@/pages/DefaultPage.tsx";
import Register from "@/pages/Register.tsx";
import {MainPage} from "@/pages/MainPage.tsx";
import {TaskCreateForm} from "@/components/TaskCreateForm.tsx";

export const privateRoutes = [
    { path: '/main', component: MainPage },
    { path: '/create', component: TaskCreateForm },
];

export const publicRoutes = [
    { path: '*', component: DefaultPage },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];