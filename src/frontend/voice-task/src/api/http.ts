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
        const errorData = error.response?.data?.error;

        // 1. Обработка 401 (Unauthorized)
        if (error.response?.status === 401) {
            useAuthStore.getState().logout();
            let message = errorData?.message || "Произошла ошибка";
            if (errorData?.message === "Токен доступа истёк или невалиден") {
                message = "Сессия истекла";
            }
            toast.error(message);
            return Promise.reject(error);
        }

        // 2. Проверка наличия специфических ошибок по полям (Fluent Validation)
        if (errorData?.fields && Object.keys(errorData.fields).length > 0) {
            Object.entries(errorData.fields).forEach(([, message]) => {
                toast.error(String(message));
            });
        }
        // 3. Если ошибок по полям нет, но есть общее сообщение
        else if (errorData?.message) {
            toast.error(errorData.message);
        }
        // 4. Крайний случай
        else {
            toast.error("Произошла системная ошибка");
        }

        return Promise.reject(error);
    }
);