import { useState } from 'react';
import { boardApi } from '../api/boardApi';
import { useBoardStore } from '../store/boardStore';

export function useBoards() {
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const { boards, setBoards, addBoard, removeBoard } = useBoardStore();

    const fetchBoards = async () => {
        setLoading(true);
        try {
            const data = await boardApi.getAll();
            setBoards(data);
        } catch {
            setError('Failed to load boards.');
        } finally {
            setLoading(false);
        }
    };

    const createBoard = async (name: string) => {
        const board = await boardApi.create(name);
        addBoard(board);
        return board;
    };

    const deleteBoard = async (id: string) => {
        await boardApi.delete(id);
        removeBoard(id);
    };

    return { boards, loading, error, fetchBoards, createBoard, deleteBoard };
}