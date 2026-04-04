import { useState, type ChangeEvent, type FormEvent } from "react";
import type { LoginFormData } from "../types/auth";
import { authApi } from "@/api/auth.api.ts";
import { useAuthStore } from "@/stores/authStore.ts";
import { Input } from "@/components/ui/input.tsx";
import { Button } from "@/components/ui/button.tsx";
import { Label } from "@/components/ui/label.tsx";
import {Link} from "react-router-dom";

const Login = () => {
    const setAuthData = useAuthStore((state) => state.setAuthData);
    const [form, setForm] = useState<LoginFormData>({ username: "", password: "" });
    const [error, setError] = useState<string>("");

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError("");

        try {
            const response = await authApi.login(form);
            const token = response.data.data;

            if (response.status !== 200) {
                setError("Ошибка входа");
                return;
            }

            setAuthData({ accessToken: token });
            alert("Успешный вход!");
        } catch (error) {
            console.log(error);
            setError("Сетевая ошибка");
        }
    };

    return (
        <div className="flex flex-col justify-center items-center min-h-screen bg-background px-4">
            <h2 className="text-2xl font-semibold mb-6">Вход</h2>

            <form onSubmit={handleSubmit} className="flex flex-col w-full max-w-xs gap-4">
                <div className="flex flex-col gap-1">
                    <Label htmlFor="username">Имя пользователя</Label>
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

                {error && <div className="text-red-500 text-sm text-center">{error}</div>}

                <Button type="submit" className="w-full">
                    Войти
                </Button>

                <div className="mt-4 text-sm text-gray-500">
                    Еще не зарегистрированы?{" "}
                    <Link to="/register" className="text-blue-500 hover:underline">
                        Зарегистрироваться
                    </Link>
                </div>
            </form>
        </div>
    );
};

export default Login;