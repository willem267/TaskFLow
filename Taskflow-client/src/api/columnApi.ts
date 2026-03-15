import api from './axios';
import type { Column } from '../types';

export const columnApi = {
    create: async (boardId: string, name: string) => {
        const { data } = await api.post<Column>('/columns', { boardId, name });
        return data;
    },

    update: async (id: string, name: string) => {
        await api.patch(`/columns/${id}`, { name });
    },

    delete: async (id: string) => {
        await api.delete(`/columns/${id}`);
    },
};