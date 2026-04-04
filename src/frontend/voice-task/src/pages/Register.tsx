import { useState, type ChangeEvent, type SubmitEvent } from "react";
import type {RegisterFormData} from "../types/auth";
import {authApi} from "@/api/auth.api.ts";
import {Input} from "@/components/ui/input.tsx";
import {Label} from "@/components/ui/label.tsx";
import {Button} from "@/components/ui/button.tsx";
import {Card} from "@/components/ui/card.tsx";

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
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value,
        }));
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

            if (response.status != 201) {
                setError(data.error.message || "Ошибка регистрации");
                return;
            }

            alert("Регистрация успешна!");
        } catch (err) {
            console.error(err);
            setError("Сетевая ошибка");
        }
    };

    return (
        <Card >
            <form onSubmit={handleSubmit}>
                <div className="flex flex-col gap-4">
                    <Card>
                        <Label>Уникальное имя пользователя</Label>
                        <Input
                            name="username"
                            type="text"
                            value={form.username}
                            onChange={handleChange}
                            required
                        />
                    </Card>

                    <Card>
                        <Label>Ваше имя</Label>
                        <Input
                            name="name"
                            type="text"
                            value={form.name}
                            onChange={handleChange}
                            required
                        />
                    </Card>

                    <Card>
                        <Label>Email</Label>
                        <Input
                            name="email"
                            type="email"
                            value={form.email}
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

                    <Card>
                        <Label>Подтвердите пароль</Label>
                        <Input
                            name="confirm"
                            type="password"
                            value={form.confirm}
                            onChange={handleChange}
                            required
                        />
                    </Card>

                    {error && <div color="red.500">{error}</div>}

                    <Button type="submit">
                        Зарегистрироваться
                    </Button>
                </div>
            </form>
        </Card>
    );
};

export default Register;
