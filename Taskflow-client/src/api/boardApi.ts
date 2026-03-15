import api from './axios';
import type { BoardSummary, BoardDetail } from '../types';

export const boardApi = {
    getAll: async () => {
        const { data } = await api.get<BoardSummary[]>('/boards');
        return data;
    },

    getById: async (id: string) => {
        const { data } = await api.get<BoardDetail>(`/boards/${id}`);
        return data;
    },

    create: async (name: string) => {
        const { data } = await api.post<BoardSummary>('/boards', { name });
        return data;
    },

    update: async (id: string, name: string) => {
        await api.patch(`/boards/${id}`, { name });
    },

    delete: async (id: string) => {
        await api.delete(`/boards/${id}`);
    },
};