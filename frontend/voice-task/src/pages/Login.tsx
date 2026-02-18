import { useState, type ChangeEvent, type SubmitEvent } from "react";
import {
    Box,
    Button,
    Field,
    Input,
    Stack,
    Text,
} from "@chakra-ui/react";
import type { LoginFormData } from "../types/auth";
import {authApi} from "@/api/auth.api.ts";

const Login = () => {
    const [form, setForm] = useState<LoginFormData>({
        username: "",
        password: "",
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
        setLoading(true);

        try {
            const response = await authApi.login(form)
            const token = response.data.data;

            if (response.status != 200) {
                setError("Ошибка входа");
                return;
            }

            localStorage.setItem("access_token", token);
            alert("Успешный вход!");
        } catch {
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
                        <Field.Label>Имя пользователя</Field.Label>
                        <Input
                            name="username"
                            type="text"
                            value={form.username}
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

                    {error && <Text color="red.500">{error}</Text>}

                    <Button type="submit" colorScheme="teal" loading={loading}>
                        Войти
                    </Button>
                </Stack>
            </form>
        </Box>
    );
};

export default Login;
