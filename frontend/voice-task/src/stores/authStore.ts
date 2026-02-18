import { create } from 'zustand';
import { devtools } from 'zustand/middleware';
import type { AuthResponse } from '@/types/auth';

type AuthStoreState = {
    id: number | null;
    isAuth: boolean;
    accessToken: string | null;
};

type AuthStoreActions = {
    setAuthData: (authData: AuthResponse) => void;
    logout: () => void;
};

type AuthStore = AuthStoreState & AuthStoreActions;

export const useAuthStore = create<AuthStore>()(
    devtools((set) => ({
        isAuth: false,
        accessToken: null,

        setAuthData: (authData: AuthResponse) => {
            const { accessToken } = authData;
            localStorage.setItem('access_token', authData.accessToken);
            set({
                isAuth: true,
                accessToken,
            });
        },

        logout: () => {
            localStorage.removeItem('access_token');
            set({
                isAuth: false,
                accessToken: null,
            });
        },
    }))
);