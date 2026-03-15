import { useState } from 'react';
import { boardApi } from '../api/boardApi';
import { columnApi } from '../api/columnApi';
import { taskApi } from '../api/taskApi';
import { useBoardStore } from '../store/boardStore';
import type { Member } from '../types';

interface UpdateTaskPayload {
    title: string;
    description?: string;
    dueDate?: string;
    assigneeId?: string;
}

export function useBoard(boardId: string) {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const {
        currentBoard,
        setCurrentBoard,
        addColumn,
        removeColumn,
        addTask,
        updateTask,
        removeTask,
        moveTask,
    } = useBoardStore();

    const fetchBoard = async () => {
        setLoading(true);
        try {
            const data = await boardApi.getById(boardId);
            setCurrentBoard(data);
        } catch {
            setError('Failed to load board.');
        } finally {
            setLoading(false);
        }
    };

    const createColumn = async (name: string) => {
        const column = await columnApi.create(boardId, name);
        addColumn(column);
    };

    const deleteColumn = async (columnId: string) => {
        await columnApi.delete(columnId);
        removeColumn(columnId);
    };

    const createTask = async (columnId: string, title: string, description?: string) => {
        const task = await taskApi.create(columnId, title, description);
        addTask(columnId, task);
    };

    const handleUpdateTask = async (taskId: string, payload: UpdateTaskPayload, members: Member[]) => {
        await taskApi.update(taskId, payload);

        // Tìm task hiện tại trong store
        const task = currentBoard?.columns
            .flatMap((c) => c.tasks)
            .find((t) => t.id === taskId);

        if (!task) return;

        // Map assigneeId sang assignee object
        const assignee = payload.assigneeId
            ? members.find((m) => m.userId === payload.assigneeId)
            : undefined;

        // Cập nhật store
        updateTask({
            ...task,
            title: payload.title,
            description: payload.description,
            dueDate: payload.dueDate,
            assignee: assignee
                ? {
                    id: assignee.userId,
                    name: assignee.name,
                }
                : undefined,
        });
    };

    const deleteTask = async (taskId: string) => {
        await taskApi.delete(taskId);
        removeTask(taskId);
    };

    const handleMoveTask = async (
        taskId: string,
        targetColumnId: string,
        previousTaskId: string | null,
        nextTaskId: string | null,
        newPosition: number
    ) => {
        moveTask(taskId, targetColumnId, newPosition);

        try {
            await taskApi.move(taskId, targetColumnId, previousTaskId, nextTaskId);
        } catch {
            await fetchBoard();
        }
    };

    return {
        board: currentBoard,
        loading,
        error,
        fetchBoard,
        createColumn,
        deleteColumn,
        createTask,
        handleUpdateTask,
        deleteTask,
        updateTask,
        handleMoveTask,
    };
}