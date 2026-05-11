import { useCallback, useEffect, useState, useMemo } from "react";
import { taskApi, GetTasksQuery } from "@/api/task.api";
import { Button } from "@/components/ui/button.tsx";
import { useNavigate } from "react-router-dom";
import { Plus, Calendar, AlertCircle, Inbox } from "lucide-react";
import type { Task } from "@/types/task";
import { TaskCard } from "@/components/TaskCard.tsx";
import { TaskSheet } from "@/components/TaskSheet.tsx";
import { Skeleton } from "@/components/ui/skeleton.tsx";
import { format, isToday, isBefore, startOfDay, parseISO } from "date-fns";
import { ru } from "date-fns/locale";

export const TaskManager = () => {
    const [tasks, setTasks] = useState<Task[]>([]);
    const [currentTime, setCurrentTime] = useState(new Date());
    const [error, setError] = useState("");
    const [selectedTask, setSelectedTask] = useState<Task | null>(null);
    const [open, setOpen] = useState(false);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const timer = setInterval(() => setCurrentTime(new Date()), 60000);
        return () => clearInterval(timer);
    }, []);

    const fetchTasks = async () => {
        setLoading(true);
        try {
            const res = await taskApi.getTasks(new GetTasksQuery());
            setTasks(res.data.data.tasks);
        } catch (err: any) {
            setError(err.message || "Ошибка загрузки списка задач");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchTasks();
    }, []);

    const groupedTasks = useMemo(() => {
        const today = startOfDay(currentTime);

        return {
            overdue: tasks.filter(t =>
                t.dueDate &&
                isBefore(parseISO(t.dueDate), today) &&
                t.status !== "done" && t.status !== "canceled"
            ),
            today: tasks.filter(t => t.dueDate && isToday(parseISO(t.dueDate))),
            upcoming: tasks.filter(t =>
                t.dueDate &&
                !isToday(parseISO(t.dueDate)) &&
                !isBefore(parseISO(t.dueDate), today)
            ),
            noDate: tasks.filter(t => !t.dueDate)
        };
    }, [tasks, currentTime]);

    const handleOpen = async (taskId: string) => {
        try {
            const { data } = await taskApi.getTaskById(taskId);
            setSelectedTask(data.data);
            setOpen(true);
        } catch (err) {
            console.error("Ошибка при открытии задачи:", err);
        }
    };

    const handleToggle = async (task: Task) => {
        const newStatus = task.status === "done" ? "inProgress" : "done";
        try {
            await taskApi.updateTask(task.taskId, { status: newStatus });
            setTasks(prev => prev.map(t =>
                t.taskId === task.taskId ? { ...t, status: newStatus } : t
            ));
        } catch (error) {
            console.error("Ошибка обновления статуса:", error);
        }
    };

    const handleEdit = useCallback((taskId: string) => {
        navigate(`/tasks/${taskId}/edit`);
    }, [navigate]);

    const SectionHeader = ({ title, count, variant = "default" }: { title: string, count: number, variant?: "default" | "danger" }) => (
        <div className="flex items-center gap-2 mb-4 mt-8 first:mt-0">
            <h3 className={`text-xs font-bold uppercase tracking-widest ${variant === "danger" ? "text-red-500" : "text-gray-400"}`}>
                {title}
            </h3>
            <div className="h-[1px] flex-1 bg-gray-100" />
            <span className="text-[10px] bg-gray-100 text-gray-500 px-2 py-0.5 rounded-md font-bold">
                {count}
            </span>
        </div>
    );

    return (
        <div className="max-w-2xl mx-auto p-4 md:p-8 pb-32">
            {/* Хедер с датой и временем */}
            <header className="flex justify-between items-center mb-10">
                <div>
                    <h1 className="text-3xl font-black tracking-tight capitalize">
                        {format(currentTime, "eeee", { locale: ru })}
                    </h1>
                    <div className="flex items-center gap-3 text-sm text-gray-500 mt-1">
                        <span className="flex items-center gap-1">
                            <Calendar className="w-3.5 h-3.5" />
                            {format(currentTime, "d MMMM y", { locale: ru })}
                        </span>
                    </div>
                </div>
                {loading && <Skeleton className="h-8 w-8 rounded-full animate-pulse" />}
            </header>

            {error && (
                <div className="bg-red-50 text-red-600 p-4 rounded-2xl mb-6 flex items-center gap-3 text-sm border border-red-100">
                    <AlertCircle className="w-5 h-5" /> {error}
                </div>
            )}

            <main className="space-y-2">
                {loading && tasks.length === 0 ? (
                    Array.from({ length: 4 }).map((_, i) => (
                        <Skeleton key={i} className="h-20 w-full rounded-2xl mb-2" />
                    ))
                ) : tasks.length === 0 ? (
                    <div className="flex flex-col items-center justify-center py-20 text-gray-400">
                        <Inbox className="w-12 h-12 mb-4 opacity-20" />
                        <p>Список задач пуст</p>
                    </div>
                ) : (
                    <>
                        {/* Группа: Просрочено */}
                        {groupedTasks.overdue.length > 0 && (
                            <section>
                                <SectionHeader title="Просрочено" count={groupedTasks.overdue.length} variant="danger" />
                                <div className="space-y-2">
                                    {groupedTasks.overdue.map(task => (
                                        <TaskCard key={task.taskId} task={task} onOpen={() => handleOpen(task.taskId)} onToggle={() => handleToggle(task)} />
                                    ))}
                                </div>
                            </section>
                        )}

                        {/* Группа: Сегодня */}
                        <section>
                            <SectionHeader title="На сегодня" count={groupedTasks.today.length} />
                            <div className="space-y-2">
                                {groupedTasks.today.length > 0 ? (
                                    groupedTasks.today.map(task => (
                                        <TaskCard key={task.taskId} task={task} onOpen={() => handleOpen(task.taskId)} onToggle={() => handleToggle(task)} />
                                    ))
                                ) : (
                                    <div className="text-sm text-gray-400 text-center py-4 border-2 border-dashed rounded-2xl border-gray-50">
                                        На сегодня планов нет
                                    </div>
                                )}
                            </div>
                        </section>

                        {/* Группа: Предстоящие */}
                        {groupedTasks.upcoming.length > 0 && (
                            <section>
                                <SectionHeader title="В планах" count={groupedTasks.upcoming.length} />
                                <div className="space-y-2">
                                    {groupedTasks.upcoming.map(task => (
                                        <TaskCard key={task.taskId} task={task} onOpen={() => handleOpen(task.taskId)} onToggle={() => handleToggle(task)} />
                                    ))}
                                </div>
                            </section>
                        )}

                        {/* Группа: Без даты */}
                        {groupedTasks.noDate.length > 0 && (
                            <section>
                                <SectionHeader title="Без срока" count={groupedTasks.noDate.length} />
                                <div className="space-y-2 opacity-90">
                                    {groupedTasks.noDate.map(task => (
                                        <TaskCard key={task.taskId} task={task} onOpen={() => handleOpen(task.taskId)} onToggle={() => handleToggle(task)} />
                                    ))}
                                </div>
                            </section>
                        )}
                    </>
                )}
            </main>

            {/* Боковая панель деталей задачи */}
            <TaskSheet
                task={selectedTask}
                open={open}
                onOpenChange={setOpen}
                onOpenTask={async (t) => {
                    setOpen(false);
                    await handleOpen(t);
                }}
                onToggleSubtask={async (taskId, status) => {
                    await taskApi.updateTask(taskId, {
                        status: status === "done" ? "inProgress" : "done",
                    });
                    if (selectedTask) await handleOpen(selectedTask.taskId);
                }}
                onEdit={handleEdit}
                onDelete={async (taskId) => {
                    await taskApi.deleteTask(taskId);
                    setOpen(false);
                    await fetchTasks();
                }}
            />

            <div className="fixed bottom-16 right-4 flex max-w-md justify-end">
                <Button
                    onClick={() => navigate("/create/voice")}
                    className="relative rounded-full w-14 h-14 p-0 flex items-center justify-center shadow-lg bg-black text-white hover:bg-gray-600"
                >
                    <Plus className="w-10 h-10" />
                </Button>
            </div>
        </div>
    );
};