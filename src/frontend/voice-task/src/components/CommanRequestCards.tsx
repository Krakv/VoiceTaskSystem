import { Card, CardContent } from "@/components/ui/card";
import { cn } from "@/lib/utils";
import type { TaskCreatePayload, TaskQueryPayload } from "@/types/commandRequest";
import { Badge } from "@/components/ui/badge";
import { Check, Loader2, Pencil, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import {useState} from "react";
import {Label} from "@/components/ui/label.tsx";

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
    return <Badge variant="outline" className={cn("text-[10px] border font-semibold", p.className)}>{p.label}</Badge>;
}

function formatDate(iso?: string | null) {
    if (!iso) return "—";
    try {
        return new Intl.DateTimeFormat("ru-RU", { day: "numeric", month: "long" }).format(new Date(iso));
    } catch { return iso; }
}

// ─── Result cards ─────────────────────────────────────────────────────────────

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
        <Card className="border-2 border-blue-400 rounded-2xl overflow-hidden">
            <CardContent className="p-4 space-y-4">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold">
                        {intent === "taskUpdate" ? "Обновление задачи" : "Новая задача"}
                    </span>
                    <Badge variant="secondary" className="bg-blue-100 text-blue-700 border-blue-200 text-[10px] border">РАСПОЗНАНО</Badge>
                </div>

                <div className="grid grid-cols-2 gap-3 text-sm">
                    <div className="col-span-2">
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
                            <div className="text-xs leading-snug p-2 bg-muted/50 rounded-lg">{data.description}</div>
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
                    <Button variant="outline" size="icon" className="h-9 w-9 shrink-0 hover:bg-red-50 hover:text-red-600" onClick={onDiscard} disabled={loading}>
                        <X className="w-3.5 h-3.5" />
                    </Button>
                </div>
            </CardContent>
        </Card>
    );
}

export function UpdateCard({
                               data,
                               onConfirm,
                               onEdit,
                               onDiscard,
                               loading,
                           }: {
    data: any;
    onConfirm: (selectedTaskId?: string) => void;
    onEdit: () => void;
    onDiscard: () => void;
    loading: boolean;
}) {
    const tasks = data.tasks || [];
    const [selectedTaskId, setSelectedTaskId] = useState<string>(tasks[0]?.taskId || "");

    const selectedTask = tasks.find((t: any) => t.taskId === selectedTaskId);

    return (
        <Card className="border-2 border-amber-400 rounded-2xl overflow-hidden shadow-md">
            <CardContent className="p-4 space-y-4">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold">Обновление задачи</span>
                    <Badge className="bg-amber-100 text-amber-700 border-amber-200 text-[10px]">ИЗМЕНЕНИЕ</Badge>
                </div>

                {/* ВЫБОР ЗАДАЧИ (если их больше одной) */}
                <div className="space-y-1.5">
                    <Label className="text-[10px] uppercase text-muted-foreground font-bold">Какую задачу обновляем?</Label>
                    <select
                        value={selectedTaskId}
                        onChange={(e) => setSelectedTaskId(e.target.value)}
                        className="w-full bg-muted/50 border rounded-lg p-2 text-sm font-medium focus:outline-none focus:ring-2 focus:ring-amber-500"
                    >
                        {tasks.map((t: any) => (
                            <option key={t.taskId} value={t.taskId}>
                                {t.title} ({formatDate(t.dueDate)})
                            </option>
                        ))}
                    </select>
                </div>

                <div className="p-3 rounded-xl bg-amber-50/50 border border-amber-100 space-y-3">
                    <div className="text-[10px] font-bold text-amber-700 uppercase tracking-wider">Новые значения:</div>

                    <div className="grid grid-cols-2 gap-3 text-sm">
                        <div className="col-span-2">
                            <div className="text-xs text-muted-foreground">Название</div>
                            <div className="font-semibold">{data.title || selectedTask?.title}</div>
                        </div>

                        <div>
                            <div className="text-xs text-muted-foreground">Приоритет</div>
                            {data.priority ? (
                                <div className="flex items-center gap-1.5 text-amber-700">
                                    <PriorityBadge priority={data.priority} />
                                    <span className="text-[10px]">← {PRIORITY_MAP[selectedTask?.priority]?.label}</span>
                                </div>
                            ) : (
                                <PriorityBadge priority={selectedTask?.priority} />
                            )}
                        </div>

                        <div>
                            <div className="text-xs text-muted-foreground">Дедлайн</div>
                            <div className={cn("font-semibold", data.dueDate && "text-amber-700")}>
                                {formatDate(data.dueDate || selectedTask?.dueDate)}
                            </div>
                        </div>

                        {data.status && (
                            <div className="col-span-2 border-t pt-2 mt-1">
                                <div className="text-xs text-muted-foreground">Статус</div>
                                <div className="font-semibold text-amber-700">
                                    {STATUS_MAP[data.status]} <span className="text-muted-foreground font-normal text-xs"> (был: {STATUS_MAP[selectedTask?.status]})</span>
                                </div>
                            </div>
                        )}
                    </div>
                </div>

                <div className="flex gap-2">
                    <Button
                        className="flex-1 h-10 bg-amber-500 hover:bg-amber-600 font-bold"
                        onClick={() => onConfirm(selectedTaskId)}
                        disabled={loading || !selectedTaskId}
                    >
                        {loading ? <Loader2 className="animate-spin w-4 h-4" /> : "Применить изменения"}
                    </Button>
                    <Button variant="outline" size="icon" className="h-10 w-10 shrink-0" onClick={onEdit}>
                        <Pencil className="w-4 h-4" />
                    </Button>
                    <Button variant="outline" size="icon" className="h-10 w-10 shrink-0" onClick={onDiscard}>
                        <X className="w-4 h-4" />
                    </Button>
                </div>
            </CardContent>
        </Card>
    );
}

export function DeleteCard({
                               data,
                               onConfirm,
                               onDiscard,
                               loading,
                           }: {
    data: TaskQueryPayload;
    onConfirm: () => void;
    onDiscard: () => void;
    loading: boolean;
}) {
    return (
        <Card className="border-2 border-red-400 rounded-2xl shadow-sm overflow-hidden">
            <CardContent className="p-4 space-y-4">
                <div className="flex items-center justify-between">
                    <span className="text-sm font-semibold text-red-600">Удаление задачи</span>
                    <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200 text-[10px]">ПОДТВЕРЖДЕНИЕ</Badge>
                </div>

                <div className="space-y-2">
                    <div className="text-[10px] font-bold text-muted-foreground uppercase tracking-wider">Будет удалено:</div>
                    {data.tasks.map((t) => (
                        <div key={t.taskId} className="p-3 rounded-xl bg-red-50/50 border border-red-100">
                            <div className="flex items-start justify-between gap-2">
                                <div className="text-sm font-bold text-red-900 leading-tight">{t.title}</div>
                                <PriorityBadge priority={t.priority} />
                            </div>
                            <div className="text-[10px] text-red-700/70 mt-1.5 flex gap-2 items-center font-medium">
                                <span>{STATUS_MAP[t.status] ?? t.status}</span>
                                {t.dueDate && <span>• {formatDate(t.dueDate)}</span>}
                                {t.projectName && <span>• проект: {t.projectName}</span>}
                            </div>
                        </div>
                    ))}
                </div>

                <p className="text-xs text-muted-foreground italic bg-muted/30 p-2 rounded-lg text-center">
                    Это действие необратимо. Вы уверены?
                </p>

                <div className="flex gap-2">
                    <Button variant="destructive" className="flex-1 h-9 text-sm font-bold shadow-sm" onClick={onConfirm} disabled={loading}>
                        {loading ? <Loader2 className="w-4 h-4 animate-spin" /> : "Да, удалить"}
                    </Button>
                    <Button variant="outline" className="flex-1 h-9 text-sm" onClick={onDiscard} disabled={loading}>
                        Отмена
                    </Button>
                </div>
            </CardContent>
        </Card>
    );
}

export function QueryCard({ data, onReset }: { data: TaskQueryPayload; onReset: () => void }) {
    return (
        <Card className="border-2 border-green-400 rounded-2xl shadow-sm">
            <CardContent className="p-4 space-y-3">
                <div className="flex items-center justify-between border-b pb-2">
                    <span className="text-sm font-semibold text-green-700">Найдено задач</span>
                    <Badge className="bg-green-600 text-white border-none h-5 px-2 text-xs font-bold">
                        {data.tasks.length}
                    </Badge>
                </div>
                <div className="space-y-2 max-h-60 overflow-y-auto pr-1">
                    {data.tasks.map((t) => (
                        <div key={t.taskId} className="flex flex-col p-2.5 rounded-xl bg-muted/40 border border-muted">
                            <div className="flex items-start justify-between gap-2">
                                <div className="text-sm font-medium leading-tight">{t.title}</div>
                                <PriorityBadge priority={t.priority} />
                            </div>
                            <div className="text-[10px] text-muted-foreground mt-1.5 flex gap-2 items-center">
                                <span className="font-semibold text-foreground/70">{STATUS_MAP[t.status] ?? t.status}</span>
                                {t.dueDate && <span>• {formatDate(t.dueDate)}</span>}
                                {t.projectName && <span className="italic">• {t.projectName}</span>}
                            </div>
                        </div>
                    ))}
                </div>
                <Button variant="outline" className="w-full h-9 text-sm border-green-200 text-green-700 hover:bg-green-50" onClick={onReset}>
                    Новая команда
                </Button>
            </CardContent>
        </Card>
    );
}

export function FallbackCard({ message, onRetry }: { message?: string; onRetry: () => void }) {
    return (
        <Card className="border-2 border-dashed border-muted rounded-2xl bg-muted/10">
            <CardContent className="p-6 space-y-3 text-center">
                <div className="text-sm font-semibold">Не удалось выполнить</div>
                <p className="text-xs text-muted-foreground leading-relaxed">
                    {message ?? "Попробуйте еще раз или скажите команду иначе."}
                </p>
                <Button className="w-full h-9 rounded-xl shadow-sm" onClick={onRetry}>
                    Попробовать снова
                </Button>
            </CardContent>
        </Card>
    );
}