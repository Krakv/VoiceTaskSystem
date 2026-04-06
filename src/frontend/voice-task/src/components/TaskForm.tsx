import { useState, useEffect, useCallback } from "react";
import {GetProjectsQuery, GetTasksQuery, taskApi, type TaskCreateDto} from "@/api/task.api";
import type {Task} from "@/types/task";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Combobox, ComboboxContent, ComboboxEmpty, ComboboxInput, ComboboxItem, ComboboxList } from "@/components/ui/combobox";
import { DatePickerTime } from "@/components/DatePickerTime";
import { debounce } from "lodash";
import { useNavigate } from "react-router-dom";

import type { TaskPriority, TaskStatus } from "@/types/task";

interface TaskFormProps {
    task?: Task;
    onSuccess?: () => void;
}

export const TaskForm = ({ task, onSuccess }: TaskFormProps) => {
    const [title, setTitle] = useState(task?.title || "");
    const [description, setDescription] = useState(task?.description || "");
    const [projectName, setProjectName] = useState(task?.projectName || "");
    const [projectsList, setProjectsList] = useState<string[]>([]);
    const [parentTaskName, setParentTaskName] = useState(task?.parentTask?.title || "");
    const [parentTasksList, setParentTasksList] = useState<Task[]>([]);
    const [dueDate, setDueDate] = useState(task?.dueDate || "");
    const [priority, setPriority] = useState<TaskPriority>(task?.priority || "low");
    const [status, setStatus] = useState<TaskStatus>(task?.status || "new");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const navigate = useNavigate();

    const debouncedFetchProjects = useCallback(
        debounce(async (query: string) => {
            if (!query) return setProjectsList([]);
            try {
                const res = await taskApi.getProjects(new GetProjectsQuery({ projectName: query }));
                setProjectsList(res.data.data.projects || []);
            } catch {
                setProjectsList([]);
            }
        }, 300),
        []
    );

    const debouncedFetchParentTasks = useCallback(
        debounce(async (query: string) => {
            if (!query) return setParentTasksList([]);
            try {
                const res = await taskApi.getTasks(new GetTasksQuery({ query }));
                setParentTasksList(res.data.data.tasks || []);
            } catch {
                setParentTasksList([]);
            }
        }, 300),
        []
    );

    useEffect(() => { debouncedFetchProjects(projectName); }, [projectName]);
    useEffect(() => { debouncedFetchParentTasks(parentTaskName); }, [parentTaskName]);

    const handleSubmit = async () => {
        if (!title.trim()) {
            setError("Введите заголовок задачи");
            return;
        }

        setError("");
        setLoading(true);

        const data: TaskCreateDto = {
            title,
            description: description || undefined,
            projectName: projectName || undefined,
            parentTaskId: parentTasksList.find((t) => t.title === parentTaskName)?.taskId,
            dueDate: dueDate || undefined,
            priority,
            status,
        };

        try {
            if (task) {
                await taskApi.updateTask(task.taskId, data);
            } else {
                await taskApi.createTask(data);
            }

            onSuccess?.();
            navigate(-1);
        } catch {
            setError("Ошибка сохранения задачи");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="max-w-md mx-auto p-2 rounded-2xl space-y-6 pb-16">
            <h2 className="text-lg font-bold">{task ? "Редактирование задачи" : "Создание задачи"}</h2>

            {error && <div className="text-sm text-red-500">{error}</div>}

            <div className="space-y-2">
                <Label>Заголовок</Label>
                <Input placeholder="Например: Сделать отчёт" value={title} onChange={(e) => setTitle(e.target.value)} />
            </div>

            <div className="space-y-2">
                <Label>Описание</Label>
                <Textarea placeholder="Описание задачи..." value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>

            <div className="space-y-2 relative">
                <Label>Проект</Label>
                <Input
                    placeholder="Введите название проекта"
                    value={projectName}
                    onChange={(e) => setProjectName(e.target.value)}
                    onBlur={() => setProjectsList([])}
                />
                {projectsList.length > 0 && (
                    <div className="absolute z-50 w-full rounded-xl border bg-popover shadow-md">
                        {projectsList.map((item) => (
                            <div
                                key={item}
                                className="cursor-pointer px-3 py-2 text-sm hover:bg-accent hover:text-accent-foreground"
                                onMouseDown={(e) => {
                                    e.preventDefault();
                                    setProjectName(item);
                                    setProjectsList([]);
                                }}
                            >
                                {item}
                            </div>
                        ))}
                    </div>
                )}
            </div>

            <div className="space-y-2">
                <Label>Родительская задача</Label>
                <Combobox
                    items={parentTasksList.map(t => t.title)}
                    onValueChange={(value: string | null) => setParentTaskName(value ?? "")}
                >
                    <ComboboxInput
                        placeholder="Выберите родительскую задачу"
                        onInput={(e) => setParentTaskName((e.target as HTMLInputElement).value)}
                    />
                    <ComboboxContent>
                        <ComboboxEmpty>Задача не найдена</ComboboxEmpty>
                        <ComboboxList>
                            {(item) => (
                                <ComboboxItem key={item} value={item}>{item}</ComboboxItem>
                            )}
                        </ComboboxList>
                    </ComboboxContent>
                </Combobox>
            </div>

            <div className="space-y-2">
                <Label>Дедлайн</Label>
                <DatePickerTime value={dueDate} onChange={setDueDate} />
            </div>

            <div className="space-y-2">
                <Label>Приоритет</Label>
                <Tabs value={priority} onValueChange={value => setPriority(value as TaskPriority)} className="w-full">
                    <TabsList className="grid grid-cols-3 w-full">
                        <TabsTrigger value="low" className="text-green-600">Низкий</TabsTrigger>
                        <TabsTrigger value="medium" className="text-yellow-600">Средний</TabsTrigger>
                        <TabsTrigger value="high" className="text-red-600">Высокий</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <div className="space-y-2">
                <Label>Статус</Label>
                <Tabs value={status} onValueChange={value => setStatus(value as TaskStatus)} className="w-full">
                    <TabsList className="grid grid-cols-4 w-full">
                        <TabsTrigger value="new" className="text-gray-700">Новая</TabsTrigger>
                        <TabsTrigger value="inprogress" className="text-blue-600">В работе</TabsTrigger>
                        <TabsTrigger value="done" className="text-green-700">Готово</TabsTrigger>
                        <TabsTrigger value="canceled" className="text-red-700">Отменена</TabsTrigger>
                    </TabsList>
                </Tabs>
            </div>

            <Button onClick={handleSubmit} disabled={loading} className="w-full mt-2">
                {loading ? "Сохраняем..." : task ? "Сохранить" : "Создать задачу"}
            </Button>
        </div>
    );
};