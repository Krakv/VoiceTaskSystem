import { useState, type ChangeEvent, type SubmitEvent } from "react";
import type { RegisterFormData } from "../types/auth";
import { authApi } from "@/api/auth.api.ts";
import { Input } from "@/components/ui/input.tsx";
import { Label } from "@/components/ui/label.tsx";
import { Button } from "@/components/ui/button.tsx";
import { Link } from "react-router-dom"; // если используешь react-router

const Register = () => {
    const [form, setForm] = useState<RegisterFormData>({
        username: "",
        name: "",
        email: "",
        password: "",
        confirm: "",
    });

    const [error, setError] = useState<string>("");

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    };

    const handleSubmit = async (e: SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError("");

        if (form.password !== form.confirm) {
            setError("Пароли не совпадают");
            return;
        }

        try {
            const response = await authApi.register(form);
            const data = await response.data;

            if (response.status !== 201) {
                setError(data.error?.message || "Ошибка регистрации");
                return;
            }

            alert("Регистрация успешна!");
        } catch (err) {
            console.error(err);
            setError("Сетевая ошибка");
        }
    };

    return (
        <div className="flex flex-col justify-center items-center min-h-screen bg-background px-4">
            <h2 className="text-2xl font-semibold mb-6">Регистрация</h2>

            <form onSubmit={handleSubmit} className="flex flex-col w-full max-w-xs gap-4">
                <div className="flex flex-col gap-1">
                    <Label htmlFor="username">Уникальное имя пользователя</Label>
                    <Input
                        id="username"
                        name="username"
                        type="text"
                        value={form.username}
                        onChange={handleChange}
                        required
                        className="w-full"
                    />
                </div>

                <div className="flex flex-col gap-1">
                    <Label htmlFor="name">Ваше имя</Label>
                    <Input
                        id="name"
                        name="name"
                        type="text"
                        value={form.name}
                        onChange={handleChange}
                        required
                        className="w-full"
                    />
                </div>

                <div className="flex flex-col gap-1">
                    <Label htmlFor="email">Email</Label>
                    <Input
                        id="email"
                        name="email"
                        type="email"
                        value={form.email}
                        onChange={handleChange}
                        required
                        className="w-full"
                    />
                </div>

                <div className="flex flex-col gap-1">
                    <Label htmlFor="password">Пароль</Label>
                    <Input
                        id="password"
                        name="password"
                        type="password"
                        value={form.password}
                        onChange={handleChange}
                        required
                        className="w-full"
                    />
                </div>

                <div className="flex flex-col gap-1">
                    <Label htmlFor="confirm">Подтвердите пароль</Label>
                    <Input
                        id="confirm"
                        name="confirm"
                        type="password"
                        value={form.confirm}
                        onChange={handleChange}
                        required
                        className="w-full"
                    />
                </div>

                {error && <div className="text-red-500 text-sm text-center">{error}</div>}

                <Button type="submit" className="w-full">
                    Зарегистрироваться
                </Button>
            </form>

            <div className="mt-4 text-sm text-gray-500">
                Уже зарегистрированы?{" "}
                <Link to="/login" className="text-blue-500 hover:underline">
                    Войти
                </Link>
            </div>
        </div>
    );
};

export default Register;