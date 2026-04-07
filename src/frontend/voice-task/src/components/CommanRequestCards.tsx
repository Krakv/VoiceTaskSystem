// ─── Result cards ─────────────────────────────────────────────────────────────

import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import type {TaskCreatePayload, TaskQueryPayload} from "@/types/commandRequest";
import {Badge, Check, Loader2, Pencil, X} from "lucide-react";
import {Button} from "@/components/ui/button.tsx";

// ─── Helpers ──────────────────────────────────────────────────────────────────

const PRIORITY_MAP: Record<string, { label: string; className: string }> = {
    high:   { label: "Высокий",  className: "bg-red-100 text-red-700 border-red-200" },
    medium: { label: "Средний",  className: "bg-amber-100 text-amber-700 border-amber-200" },
    low:    { label: "Низкий",   className: "bg-green-100 text-green-700 border-green-200" },
};

const STATUS_MAP: Record<string, string> = {
    new:         "Новая",
    in_progress: "В работе",
    inProgress:  "В работе",
    done:        "Готово",
    canceled:    "Отменена",
};

function PriorityBadge({ priority }: { priority?: string }) {
    if (!priority) return <span className="text-muted-foreground">—</span>;
    const p = PRIORITY_MAP[priority.toLowerCase()] ?? { label: priority, className: "bg-muted text-muted-foreground" };
    return <Badge className={cn("text-xs border font-semibold", p.className)}>{p.label}</Badge>;
}

function formatDate(iso?: string | null) {
    if (!iso) return "—";
    try {
        return new Intl.DateTimeFormat("ru-RU", { day: "numeric", month: "long" }).format(new Date(iso));
    } catch { return iso; }
}



export function CreateUpdateCard({
                              intent,
                              data,
                              onConfirm,
                              onEdit,
                              onDiscard,
                              loading,
                          }: {
    intent: "taskCreate" | "taskUpdate";
    data: TaskCreatePayload;
    onConfirm: () => void;
    onEdit: () => void;
    onDiscard: () => void;
    loading: boolean;
}) {
    return (
        <Card className="border-2 border-blue-400 rounded-2xl">
            <CardContent className="p-4 space-y-4">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold">
                        {intent === "taskUpdate" ? "Обновление задачи" : "Предпросмотр"}
                    </span>
                    <Badge className="bg-blue-100 text-blue-700 border-blue-200 text-xs border">Распознано</Badge>
                </div>

                <div className="grid grid-cols-2 gap-3 text-sm">
                    <div>
                        <div className="text-xs text-muted-foreground mb-0.5">Название</div>
                        <div className="font-semibold leading-tight">{data.title || "—"}</div>
                    </div>
                    <div>
                        <div className="text-xs text-muted-foreground mb-0.5">Дедлайн</div>
                        <div className="font-semibold">{formatDate(data.dueDate)}</div>
                    </div>
                    <div>
                        <div className="text-xs text-muted-foreground mb-0.5">Приоритет</div>
                        <PriorityBadge priority={data.priority} />
                    </div>
                    <div>
                        <div className="text-xs text-muted-foreground mb-0.5">Проект</div>
                        <div className={data.projectName ? "font-semibold" : "text-muted-foreground"}>
                            {data.projectName || "—"}
                        </div>
                    </div>
                    {data.status && (
                        <div>
                            <div className="text-xs text-muted-foreground mb-0.5">Статус</div>
                            <div className="font-semibold">{STATUS_MAP[data.status] ?? data.status}</div>
                        </div>
                    )}
                    {data.description && (
                        <div className="col-span-2">
                            <div className="text-xs text-muted-foreground mb-0.5">Описание</div>
                            <div className="text-xs leading-snug">{data.description}</div>
                        </div>
                    )}
                    {data.message && (
                        <div className="col-span-2">
                            <div className="text-xs text-muted-foreground mb-0.5">Сообщение</div>
                            <div className="text-xs text-muted-foreground italic leading-snug">{data.message}</div>
                        </div>
                    )}
                </div>

                <div className="flex gap-2">
                    <Button className="flex-1 h-9 text-sm" onClick={onConfirm} disabled={loading}>
                        {loading
                            ? <Loader2 className="w-3.5 h-3.5 animate-spin" />
                            : <><Check className="w-3.5 h-3.5 mr-1.5" />Подтвердить</>
                        }
                    </Button>
                    <Button variant="outline" size="icon" className="h-9 w-9 shrink-0" onClick={onEdit} disabled={loading}>
                        <Pencil className="w-3.5 h-3.5" />
                    </Button>
                    <Button variant="outline" size="icon" className="h-9 w-9 shrink-0" onClick={onDiscard} disabled={loading}>
                        <X className="w-3.5 h-3.5" />
                    </Button>
                </div>
            </CardContent>
        </Card>
    );
}

export function DeleteCard({
                        onConfirm,
                        onDiscard,
                        loading,
                    }: {
    onConfirm: () => void;
    onDiscard: () => void;
    loading: boolean;
}) {
    return (
        <Card className="border-2 border-red-400 rounded-2xl">
            <CardContent className="p-4 space-y-4">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold">Удаление задачи</span>
                    <Badge className="bg-red-100 text-red-700 border-red-200 text-xs border">Распознано</Badge>
                </div>
                <p className="text-sm text-muted-foreground">
                    Вы уверены, что хотите удалить задачу? Это действие необратимо.
                </p>
                <div className="flex gap-2">
                    <Button variant="destructive" className="flex-1 h-9 text-sm" onClick={onConfirm} disabled={loading}>
                        {loading
                            ? <Loader2 className="w-3.5 h-3.5 animate-spin" />
                            : <><X className="w-3.5 h-3.5 mr-1.5" />Удалить</>
                        }
                    </Button>
                    <Button variant="outline" className="h-9 px-4 text-sm" onClick={onDiscard} disabled={loading}>
                        Отмена
                    </Button>
                </div>
            </CardContent>
        </Card>
    );
}

export function QueryCard({ data, onReset }: { data: TaskQueryPayload; onReset: () => void }) {
    return (
        <Card className="border-2 border-green-400 rounded-2xl">
            <CardContent className="p-4 space-y-3">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold">Найденные задачи</span>
                    <Badge className="bg-green-100 text-green-700 border-green-200 text-xs border">
                        {data.tasks.length}
                    </Badge>
                </div>
                <div className="space-y-2 max-h-52 overflow-y-auto">
                    {data.tasks.map((t) => (
                        <div key={t.taskId} className="flex items-start justify-between gap-2 p-2.5 rounded-xl bg-muted/60">
                            <div className="min-w-0">
                                <div className="text-sm font-medium leading-tight truncate">{t.title}</div>
                                <div className="text-xs text-muted-foreground mt-0.5">
                                    {STATUS_MAP[t.status] ?? t.status}
                                    {t.dueDate ? ` · ${formatDate(t.dueDate)}` : ""}
                                    {t.projectName ? ` · ${t.projectName}` : ""}
                                </div>
                            </div>
                            <PriorityBadge priority={t.priority} />
                        </div>
                    ))}
                </div>
                <Button variant="outline" className="w-full h-9 text-sm" onClick={onReset}>
                    Новая команда
                </Button>
            </CardContent>
        </Card>
    );
}

export function FallbackCard({ message, onRetry }: { message?: string; onRetry: () => void }) {
    return (
        <Card className="border-2 border-muted rounded-2xl">
            <CardContent className="p-4 space-y-3 text-center">
                <div className="text-sm font-semibold">
                    {message ?? "Не удалось распознать команду"}
                </div>
                <p className="text-xs text-muted-foreground">
                    Попробуйте ещё раз или скажите команду чётче
                </p>
                <Button className="w-full h-9 text-sm" onClick={onRetry}>
                    Попробовать снова
                </Button>
            </CardContent>
        </Card>
    );
}