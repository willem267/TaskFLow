import { create } from 'zustand';
import type { BoardSummary, BoardDetail, Column, TaskItem, Member } from '../types';

interface BoardState {
    boards: BoardSummary[];
    currentBoard: BoardDetail | null;
    setBoards: (boards: BoardSummary[]) => void;
    addBoard: (board: BoardSummary) => void;
    removeBoard: (id: string) => void;
    setCurrentBoard: (board: BoardDetail | null) => void;
    addColumn: (column: Column) => void;
    removeColumn: (columnId: string) => void;
    addTask: (columnId: string, task: TaskItem) => void;
    updateTask: (task: TaskItem) => void;
    removeTask: (taskId: string) => void;
    moveTask: (taskId: string, targetColumnId: string, newPosition: number) => void;
    members: Member[];
    setMembers: (members: Member[]) => void;
    addMember: (member: Member) => void;
    removeMember: (userId: string) => void;

}

export const useBoardStore = create<BoardState>((set) => ({
    boards: [],
    currentBoard: null,
    members: [],

    setBoards: (boards) => set({ boards }),

    addBoard: (board) =>
        set((state) => ({ boards: [board, ...state.boards] })),

    removeBoard: (id) =>
        set((state) => ({ boards: state.boards.filter((b) => b.id !== id) })),

    setCurrentBoard: (board) => set({ currentBoard: board }),

    addColumn: (column) =>
        set((state) => {
            if (!state.currentBoard) return state;

            const alreadyExists = state.currentBoard.columns
                .some((c) => c.id === column.id);

            if (alreadyExists) return state;

            return {
                currentBoard: {
                    ...state.currentBoard,
                    columns: [...state.currentBoard.columns, column],
                },
            };
        }),

    removeColumn: (columnId) =>
        set((state) => ({
            currentBoard: state.currentBoard
                ? {
                    ...state.currentBoard,
                    columns: state.currentBoard.columns.filter((c) => c.id !== columnId),
                }
                : null,
        })),

    addTask: (columnId, task) =>
        set((state) => {
            if (!state.currentBoard) return state;

            const alreadyExists = state.currentBoard.columns
                .flatMap((c) => c.tasks)
                .some((t) => t.id === task.id);

            if (alreadyExists) return state;

            return {
                currentBoard: {
                    ...state.currentBoard,
                    columns: state.currentBoard.columns.map((c) =>
                        c.id === columnId ? { ...c, tasks: [...c.tasks, task] } : c
                    ),
                },
            };
        }),

    updateTask: (task) =>
        set((state) => ({
            currentBoard: state.currentBoard
                ? {
                    ...state.currentBoard,
                    columns: state.currentBoard.columns.map((c) => ({
                        ...c,
                        tasks: c.tasks.map((t) => (t.id === task.id ? task : t)),
                    })),
                }
                : null,
        })),

    removeTask: (taskId) =>
        set((state) => ({
            currentBoard: state.currentBoard
                ? {
                    ...state.currentBoard,
                    columns: state.currentBoard.columns.map((c) => ({
                        ...c,
                        tasks: c.tasks.filter((t) => t.id !== taskId),
                    })),
                }
                : null,
        })),

    moveTask: (taskId, targetColumnId, newPosition) =>
        set((state) => {
            if (!state.currentBoard) return state;

            let movedTask: TaskItem | null = null;

            // Tìm và xóa task khỏi column cũ
            const columns = state.currentBoard.columns.map((c) => ({
                ...c,
                tasks: c.tasks.filter((t) => {
                    if (t.id === taskId) {
                        movedTask = { ...t, position: newPosition };
                        return false;
                    }
                    return true;
                }),
            }));

            if (!movedTask) return state;

            // Thêm task vào column mới đúng vị trí
            const updatedColumns = columns.map((c) => {
                if (c.id !== targetColumnId) return c;

                const tasks = [...c.tasks, movedTask!].sort(
                    (a, b) => a.position - b.position
                );
                return { ...c, tasks };
            });

            return {
                currentBoard: { ...state.currentBoard, columns: updatedColumns },
            };
        }),
    setMembers: (members) => set({ members }),

    addMember: (member) =>
        set((state) => ({ members: [...state.members, member] })),

    removeMember: (userId) =>
        set((state) => ({
            members: state.members.filter((m) => m.userId !== userId),
        })),
}));