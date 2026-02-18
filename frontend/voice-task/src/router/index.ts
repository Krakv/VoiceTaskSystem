import Login from '@/pages/Login'
import {DefaultPage} from "@/pages/DefaultPage.tsx";
import Register from "@/pages/Register.tsx";

export const privateRoutes = [
    { path: '/defaultpage', component: DefaultPage },
];

export const publicRoutes = [
    { path: '/defaultpage', component: DefaultPage },
    { path: '/login', component: Login },
    { path: '/register', component: Register },
];