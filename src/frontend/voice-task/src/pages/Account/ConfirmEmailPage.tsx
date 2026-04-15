import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { accountApi } from "@/api/account.api";

export const ConfirmEmailPage = () => {
    const [params] = useSearchParams();
    const token = params.get("token");

    const [status, setStatus] = useState("loading");

    useEffect(() => {
        if (!token) return;

        accountApi.confirmEmail(token)
            .then(() => setStatus("success"))
            .catch(() => setStatus("error"));
    }, [token]);

    return (
        <div className="flex items-center justify-center h-screen">
            {status === "loading" && "Подтверждение..."}
            {status === "success" && "Почта подтверждена ✅"}
            {status === "error" && "Ошибка подтверждения ❌"}
        </div>
    );
};