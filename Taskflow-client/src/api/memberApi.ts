import api from './axios';
import type { Member } from '../types';

export const memberApi = {
    getAll: async (boardId: string) => {
        const { data } = await api.get<Member[]>(`/boards/${boardId}/members`);
        return data;
    },

    invite: async (boardId: string, email: string) => {
        await api.post(`/boards/${boardId}/members`, { email });
    },

    remove: async (boardId: string, userId: string) => {
        await api.delete(`/boards/${boardId}/members/${userId}`);
    },
};