import { useEffect, useRef, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { accountApi } from "@/api/account.api";

export const ConfirmEmailPage = () => {
    const [params] = useSearchParams();
    const token = params.get("token");

    const [status, setStatus] = useState<"loading" | "success" | "error">("loading");

    const successRef = useRef(false);

    useEffect(() => {
        if (!token) return;

        accountApi.confirmEmail(token)
            .then(() => {
                successRef.current = true;
                setStatus("success");
            })
            .catch(() => {
                if (!successRef.current) {
                    setStatus("error");
                }
            });
    }, [token]);

    return (
        <div className="flex items-center justify-center h-screen text-center">
            {status === "loading" && "Подтверждение..."}
            {status === "success" && "Почта подтверждена ✅"}
            {status === "error" && "Ошибка подтверждения ❌"}
        </div>
    );
};