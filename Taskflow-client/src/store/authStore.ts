import { create } from 'zustand';
import type { User } from '../types';
import {authApi} from "../api/authApi.ts";

interface AuthState {
    user: User | null;
    isAuthenticated: boolean;
    setUser: (user: User | null) => void;
    clear: () => void;
    refresh: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set,get) => ({
    user: null,
    isAuthenticated: false,

    setUser: (user) => set({ user, isAuthenticated: !!user }),

    clear: () => set({ user: null, isAuthenticated: false }),
    refresh: async () => {
        try {
            const data = await authApi.refresh();
            get().setUser(data.user);
        } catch {
            get().clear();
        }
    },
}));