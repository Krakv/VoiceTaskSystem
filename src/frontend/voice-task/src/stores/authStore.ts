import { create } from 'zustand';
import { devtools, persist } from 'zustand/middleware';
import type { AuthResponse } from '@/types/auth';

type AuthStoreState = {
    isAuth: boolean;
    accessToken: string | null;
};

type AuthStoreActions = {
    setAuthData: (authData: AuthResponse) => void;
    logout: () => void;
};

type AuthStore = AuthStoreState & AuthStoreActions;

export const useAuthStore = create<AuthStore>()(
    devtools(
        persist(
            (set) => ({
                isAuth: false,
                accessToken: null,

                setAuthData: (authData: AuthResponse) => {
                    set({
                        isAuth: true,
                        accessToken: authData.accessToken,
                    });
                },

                logout: () => {
                    set({
                        isAuth: false,
                        accessToken: null,
                    });
                },
            }),
            {
                name: "auth-storage", // ключ в localStorage
            }
        )
    )
);
