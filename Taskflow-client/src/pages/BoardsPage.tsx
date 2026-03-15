import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBoards } from '../hooks/useBoards';
import { useAuthStore } from '../store/authStore';
import { authApi } from '../api/authApi';
import BoardCard from '../components/board/BoardCard';
import CreateBoardModal from '../components/board/CreateBoardModal';

export default function BoardsPage() {
    const navigate = useNavigate();
    const { boards, loading, error, fetchBoards, createBoard, deleteBoard } = useBoards();
    const user = useAuthStore((s) => s.user);
    const clearAuth = useAuthStore((s) => s.clear);
    const [showModal, setShowModal] = useState(false);

    useEffect(() => {
        fetchBoards();
    }, []);

    const handleLogout = async () => {
        await authApi.logout();
        clearAuth();
        navigate('/login');
    };

    const handleDelete = async (id: string) => {
        if (!confirm('Delete this board?')) return;
        await deleteBoard(id);
    };

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Navbar */}
            <nav className="bg-white border-b border-gray-200 px-6 py-4 flex items-center justify-between">
                <h1 className="text-xl font-bold text-blue-600">TaskFlow</h1>
                <div className="flex items-center gap-4">
                    <span className="text-sm text-gray-600">Hi, {user?.name}</span>
                    <button
                        onClick={handleLogout}
                        className="text-sm text-gray-500 hover:text-red-500 transition-colors"
                    >
                        Logout
                    </button>
                </div>
            </nav>

            {/* Content */}
            <main className="max-w-5xl mx-auto px-6 py-8">
                <div className="flex items-center justify-between mb-6">
                    <h2 className="text-2xl font-bold text-gray-800">My Boards</h2>
                    <button
                        onClick={() => setShowModal(true)}
                        className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-medium hover:bg-blue-700"
                    >
                        + New Board
                    </button>
                </div>

                {loading && (
                    <p className="text-gray-500 text-sm">Loading boards...</p>
                )}

                {error && (
                    <p className="text-red-500 text-sm">{error}</p>
                )}

                {!loading && boards.length === 0 && (
                    <div className="text-center py-16 text-gray-400">
                        <p className="text-lg">No boards yet.</p>
                        <p className="text-sm mt-1">Create your first board to get started.</p>
                    </div>
                )}

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    {boards.map((board) => (
                        <BoardCard
                            key={board.id}
                            board={board}
                            onDelete={handleDelete}
                        />
                    ))}
                </div>
            </main>

            {showModal && (
                <CreateBoardModal
                    onClose={() => setShowModal(false)}
                    onCreate={createBoard}
                />
            )}
        </div>
    );
}