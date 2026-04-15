import {api} from "@/api/http.ts";

export interface UpdateAccountDto {
    name?: string;
    email?: string;
}

export interface ChangePasswordDto {
    currentPassword: string;
    newPassword: string;
}

export const accountApi = {
    getAccount: () => api.get("/auth/me"),
    getTelegramLinkToken: () => api.get("/auth/telegram-link-token"),
    updateAccount: (data: UpdateAccountDto) => api.patch(`/auth/profile`, data),
    changePassword: (data: ChangePasswordDto) => api.patch(`/auth/change-password`, data),
    deleteAccount: () => api.delete(`/auth/account`),
    unlinkTelegram: () => api.delete(`/auth/telegram-link`),
    sendEmailVerification: () => api.post("/auth/email/send-verification"),
    confirmEmail: (token: string) => api.post("/auth/email/confirm", { token }),
};
