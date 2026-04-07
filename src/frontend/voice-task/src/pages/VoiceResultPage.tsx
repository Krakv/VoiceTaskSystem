import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Loader2 } from "lucide-react";

import { commandRequestApi } from "@/api/commandRequest.api.ts";
import type {TaskCreatePayload, TaskQueryPayload, UnknownPayload, VoiceStatusData} from "@/types/commandRequest.ts";
import {CreateUpdateCard, DeleteCard, FallbackCard, QueryCard} from "@/components/CommanRequestCards.tsx";

export const VoiceResultPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [statusData, setStatusData] = useState<VoiceStatusData | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [actionLoading, setActionLoading] = useState(false);

    useEffect(() => {
        if (!id) return;

        const interval = setInterval(async () => {
            try {
                const res = await commandRequestApi.getVoiceTaskStatus(id);

                if (res.status === 202 || res.data.error?.code === "PENDING") {
                    return;
                }

                clearInterval(interval);
                setStatusData(res.data.data);
                setLoading(false);
            } catch {
                clearInterval(interval);
                setError("Ошибка получения результата");
                setLoading(false);
            }
        }, 1500);

        return () => clearInterval(interval);
    }, [id]);

    const handleConfirm = async () => {
        setActionLoading(true);
        try {
            await commandRequestApi.confirmVoiceTask(id!);
            navigate("/");
        } catch {
            setError("Ошибка подтверждения");
        } finally {
            setActionLoading(false);
        }
    };

    const handleDiscard = async () => {
        await commandRequestApi.deleteVoiceTask(id!);
        navigate("/");
    };

    const handleEdit = () => {
        navigate(`/create/voice/${id}/edit`);
    };

    // ─── UI ─────────────────────────────

    if (loading) {
        return (
            <div className="flex flex-col items-center justify-center h-full gap-4">
                <Loader2 className="w-10 h-10 animate-spin" />
                <p className="text-sm text-muted-foreground">Обрабатываем запись…</p>
            </div>
        );
    }

    if (error || !statusData) {
        return <div className="text-center">{error || "Ошибка"}</div>;
    }

    return (
        <div className="p-4 space-y-4">

            {(statusData.intent === "taskCreate" || statusData.intent === "taskUpdate") && (
                <CreateUpdateCard
                    intent={statusData.intent}
                    data={statusData.payload as TaskCreatePayload}
                    onConfirm={handleConfirm}
                    onEdit={handleEdit}
                    onDiscard={handleDiscard}
                    loading={actionLoading}
                />
            )}

            {statusData.intent === "taskDelete" && (
                <DeleteCard
                    onConfirm={handleConfirm}
                    onDiscard={handleDiscard}
                    loading={actionLoading}
                />
            )}

            {statusData.intent === "taskQuery" && (
                <QueryCard
                    data={statusData.payload as TaskQueryPayload}
                    onReset={() => navigate("/create/voice")}
                />
            )}

            {(statusData.intent === "unknown" || statusData.intent === "ambiguous") && (
                <FallbackCard
                    message={(statusData.payload as UnknownPayload)?.message}
                    onRetry={() => navigate("/create/voice")}
                />
            )}
        </div>
    );
};