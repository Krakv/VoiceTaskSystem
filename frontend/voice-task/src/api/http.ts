import axios from "axios";
import { useAuthStore } from "@/stores/authStore";

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
