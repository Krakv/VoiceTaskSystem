import { useState, type ChangeEvent, type SubmitEvent } from "react";
import type { LoginFormData } from "../types/auth";
import {authApi} from "@/api/auth.api.ts";
import {useAuthStore} from "@/stores/authStore.ts";
import {Input} from "@/components/ui/input.tsx";
import {Button} from "@/components/ui/button.tsx";
import {Card} from "@/components/ui/card.tsx";
import {Label} from "@/components/ui/label.tsx";

const Login = () => {
    const setAuthData = useAuthStore((state) => state.setAuthData);
    const [form, setForm] = useState<LoginFormData>({
        username: "",
        password: "",
    });

    const [error, setError] = useState<string>("");

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value,
        }));
    };

    const handleSubmit = async (e: SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError("");

        try {
            const response = await authApi.login(form)
            const token = response.data.data;

            if (response.status != 200) {
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
        <Card >
            <form onSubmit={handleSubmit}>
                <div className="flex flex-col gap-4">
                    <Card>
                        <Label>Имя пользователя</Label>
                        <Input
                            name="username"
                            type="text"
                            value={form.username}
                            onChange={handleChange}
                            required
                        />
                    </Card>

                    <Card>
                        <Label>Пароль</Label>
                        <Input
                            name="password"
                            type="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                        />
                    </Card>

                    {error && <div color="red.500">{error}</div>}

                    <Button type="submit" >
                        Войти
                    </Button>
                </div>
            </form>
        </Card>
    );
};

export default Login;
