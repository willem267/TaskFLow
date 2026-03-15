import api, { setAccessToken } from './axios';
import type { AuthResponse } from '../types';

export const authApi = {
    register: async (email: string, password: string, name: string) => {
        const { data } = await api.post<AuthResponse>('/auth/register', {
            email,
            password,
            name,
        });
        setAccessToken(data.accessToken);
        return data;
    },

    login: async (email: string, password: string) => {
        const { data } = await api.post<AuthResponse>('/auth/login', {
            email,
            password,
        });
        setAccessToken(data.accessToken);
        return data;
    },

    logout: async () => {
        await api.post('/auth/logout');
        setAccessToken(null);
    },

    refresh: async () => {
        const { data } = await api.post<AuthResponse>('/auth/refresh', {}, {withCredentials:true});
        setAccessToken(data.accessToken);
        return data;
    },
};