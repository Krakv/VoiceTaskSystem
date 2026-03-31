import { useState, type ChangeEvent, type SubmitEvent } from "react";
import {
    Box,
    Button,
    Input,
    Stack,
    Text,
    Field
} from "@chakra-ui/react";
import type {RegisterFormData} from "../types/auth";
import {authApi} from "@/api/auth.api.ts";

const Register = () => {
    const [form, setForm] = useState<RegisterFormData>({
        username: "",
        name: "",
        email: "",
        password: "",
        confirm: "",
    });

    const [error, setError] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(false);

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

        setLoading(true);

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
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box maxW="md" mx="auto" mt="10">
            <form onSubmit={handleSubmit}>
                <Stack gap="4">
                    <Field.Root>
                        <Field.Label>Уникальное имя пользователя</Field.Label>
                        <Input
                            name="username"
                            type="text"
                            value={form.username}
                            onChange={handleChange}
                            required
                        />
                    </Field.Root>

                    <Field.Root>
                        <Field.Label>Ваше имя</Field.Label>
                        <Input
                            name="name"
                            type="text"
                            value={form.name}
                            onChange={handleChange}
                            required
                        />
                    </Field.Root>

                    <Field.Root>
                        <Field.Label>Email</Field.Label>
                        <Input
                            name="email"
                            type="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                        />
                    </Field.Root>

                    <Field.Root>
                        <Field.Label>Пароль</Field.Label>
                        <Input
                            name="password"
                            type="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                        />
                    </Field.Root>

                    <Field.Root>
                        <Field.Label>Подтвердите пароль</Field.Label>
                        <Input
                            name="confirm"
                            type="password"
                            value={form.confirm}
                            onChange={handleChange}
                            required
                        />
                    </Field.Root>

                    {error && <Text color="red.500">{error}</Text>}

                    <Button type="submit" colorScheme="teal" loading={loading}>
                        Зарегистрироваться
                    </Button>
                </Stack>
            </form>
        </Box>
    );
};

export default Register;
