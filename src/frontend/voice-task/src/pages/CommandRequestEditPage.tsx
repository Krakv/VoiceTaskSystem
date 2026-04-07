import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { CommandRequestForm } from "@/components/CommandRequestForm";
import { commandRequestApi } from "@/api/commandRequest.api";
import type { TaskCreatePayload, VoiceStatusData } from "@/types/commandRequest";
import type {TaskPriority, TaskStatus} from "@/types/task.ts";

export const CommandRequestEditPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [data, setData] = useState<TaskCreatePayload | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (!id) return;

        const fetchData = async () => {
            try {
                const res = await commandRequestApi.getVoiceTaskStatus(id);
                const status: VoiceStatusData = res.data.data;

                if (status.intent !== "taskCreate" && status.intent !== "taskUpdate") {
                    setError("Команда не поддерживает редактирование");
                    return;
                }

                const payload = status.payload as TaskCreatePayload;

                const normalizedData: TaskCreatePayload = {
                    ...payload,
                    status: payload.status?.toLowerCase() as TaskStatus,
                    priority: payload.priority?.toLowerCase() as TaskPriority,
                };

                setData(normalizedData);
            } catch {
                setError("Ошибка загрузки команды");
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [id]);

    if (loading) return <div className="p-4">Загрузка...</div>;
    if (error) return <div className="p-4 text-red-500">{error}</div>;
    if (!data) return null;

    return (
        <CommandRequestForm
            commandRequestId={id!}
            initialData={data}
            onSuccess={() => navigate(-1)}
        />
    );
};