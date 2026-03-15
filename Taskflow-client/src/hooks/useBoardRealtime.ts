import { useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { getAccessToken } from '../api/axios';
import { useBoardStore } from '../store/boardStore';
import { boardApi } from '../api/boardApi';
import type { TaskItem, Column } from '../types';

const BoardEvents = {
    TaskCreated: 'task:created',
    TaskUpdated: 'task:updated',
    TaskDeleted: 'task:deleted',
    TaskMoved: 'task:moved',
    ColumnCreated: 'column:created',
    ColumnUpdated: 'column:updated',
    ColumnDeleted: 'column:deleted',
} as const;

export function useBoardRealtime(boardId: string) {
    const {
        addTask,
        updateTask,
        removeTask,
        moveTask,
        addColumn,
        removeColumn,
        setCurrentBoard,
    } = useBoardStore();

    useEffect(() => {
        // Tạo connection mới mỗi lần vào board
        const connection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:7000/hubs/board', {
                accessTokenFactory: () => getAccessToken() ?? '',
            })
            .withAutomaticReconnect()
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        const start = async () => {
            try {
                await connection.start();
                await connection.invoke('JoinBoard', boardId);
            } catch (err) {
                console.error('SignalR connection failed:', err);
            }
        };

        // Đăng ký event handlers
        connection.on(BoardEvents.TaskCreated, (task: TaskItem) => {
            if (task.columnId) addTask(task.columnId, task);
        });

        connection.on(BoardEvents.TaskUpdated, (task: TaskItem) => {
            updateTask(task);
        });

        connection.on(BoardEvents.TaskDeleted, (payload: { taskId: string }) => {
            removeTask(payload.taskId);
        });

        connection.on(
            BoardEvents.TaskMoved,
            (payload: { taskId: string; targetColumnId: string; newPosition: number }) => {
                moveTask(payload.taskId, payload.targetColumnId, payload.newPosition);
            }
        );

        connection.on(BoardEvents.ColumnCreated, (column: Column) => {
            addColumn(column);
        });

        connection.on(BoardEvents.ColumnUpdated, (column: Column) => {
            useBoardStore.setState((state) => ({
                currentBoard: state.currentBoard
                    ? {
                        ...state.currentBoard,
                        columns: state.currentBoard.columns.map((c) =>
                            c.id === column.id ? { ...c, name: column.name } : c
                        ),
                    }
                    : null,
            }));
        });

        connection.on(BoardEvents.ColumnDeleted, (payload: { columnId: string }) => {
            removeColumn(payload.columnId);
        });

        // Fetch lại board sau khi reconnect
        connection.onreconnected(async () => {
            try {
                await connection.invoke('JoinBoard', boardId);
                const data = await boardApi.getById(boardId);
                setCurrentBoard(data);
            } catch (err) {
                console.error('Reconnect sync failed:', err);
            }
        });

        start();

        // Cleanup khi unmount
        return () => {
            connection.invoke('LeaveBoard', boardId).catch(() => {});
            connection.stop();
        };
    }, [boardId]);
}