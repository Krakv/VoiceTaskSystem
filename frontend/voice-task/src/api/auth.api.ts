import { api } from "./http";
import { ENDPOINTS } from "./endpoints";

interface LoginDto {
    username: string;
    password: string;
}

interface RegisterDto {
    username: string;
    name: string;
    email: string;
    password: string;
}

export const authApi = {
    login: (data: LoginDto) =>
        api.post(ENDPOINTS.AUTH.LOGIN, data),

    register: (data: RegisterDto) =>
        api.post(ENDPOINTS.AUTH.REGISTER, data),
};
