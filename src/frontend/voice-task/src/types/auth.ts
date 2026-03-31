export interface RegisterFormData {
    name: string;
    username: string;
    email: string;
    password: string;
    confirm: string;
}

export interface LoginFormData {
    username: string;
    password: string;
}

export interface AuthResponse {
    accessToken: string;
}
