import axios from "axios";
import { useAuthStore } from "@/stores/authStore";
import { toast } from "sonner";

export const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "/api/v1",
    headers: {
        "Content-Type": "application/json",
    },
});

api.interceptors.request.use((config) => {
    const accessToken = useAuthStore.getState().accessToken;

    if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
});

api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            useAuthStore.getState().logout();
            toast.error(error.response.data.error.message);
        } else if (error.response?.data?.error?.message) {
            toast.error(error.response.data.error.message);
        } else {
            toast.error("Неизвестная ошибка");
        }

        return Promise.reject(error);
    }
);
