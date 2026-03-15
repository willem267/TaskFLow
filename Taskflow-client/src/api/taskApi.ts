import api from './axios';
import type { TaskItem } from '../types';

export const taskApi = {
    create: async (columnId: string, title: string, description?: string) => {
        const { data } = await api.post<TaskItem>('/tasks', {
            columnId,
            title,
            description,
        });
        return data;
    },

    update: async (id: string, payload: Partial<TaskItem>) => {
        await api.patch(`/tasks/${id}`, payload);
    },

    move: async (
        id: string,
        targetColumnId: string,
        previousTaskId: string | null,
        nextTaskId: string | null
    ) => {
        const { data } = await api.patch<TaskItem>(`/tasks/${id}/move`, {
            targetColumnId,
            previousTaskId,
            nextTaskId,
        });
        return data;
    },

    delete: async (id: string) => {
        await api.delete(`/tasks/${id}`);
    },
};