import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useBoard } from '../hooks/useBoard';
import { useBoardRealtime } from '../hooks/useBoardRealtime';
import { useRole } from '../hooks/useRole';
import { memberApi } from '../api/memberApi';
import { useBoardStore } from '../store/boardStore';
import ColumnList from '../components/column/ColumnList';
import CreateColumnForm from '../components/column/CreateColumnForm';
import MembersPanel from '../components/board/MembersPanel';
import ConnectionStatus from '../components/ui/ConnectionStatus';

export default function BoardDetailPage() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [showMembers, setShowMembers] = useState(false);

    const {
        board, loading, error,
        fetchBoard, createColumn, deleteColumn,
        createTask, deleteTask, handleMoveTask,
    } = useBoard(id!);

    const members = useBoardStore((s) => s.members);
    const setMembers = useBoardStore((s) => s.setMembers);
    const addMember = useBoardStore((s) => s.addMember);
    const removeMemberFromStore = useBoardStore((s) => s.removeMember);
    const { isOwner, isMember } = useRole();

    useBoardRealtime(id!);

    useEffect(() => {
        fetchBoard();
        memberApi.getAll(id!).then(setMembers);
    }, [id]);

    const handleInvite = async (email: string) => {
        await memberApi.invite(id!, email);
        // Refresh member list
        const updated = await memberApi.getAll(id!);
        setMembers(updated);
    };

    const handleRemoveMember = async (userId: string) => {
        await memberApi.remove(id!, userId);
        removeMemberFromStore(userId);
    };

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center text-gray-400">
                Loading board...
            </div>
        );
    }

    if (error || !board) {
        return (
            <div className="min-h-screen flex items-center justify-center text-red-400">
                {error || 'Board not found.'}
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-600 to-blue-800">
            {/* Header */}
            <div className="px-6 py-4 flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <button
                        onClick={() => navigate('/boards')}
                        className="text-white/70 hover:text-white text-sm"
                    >
                        ← Back
                    </button>
                    <h1 className="text-white font-bold text-xl">{board.name}</h1>
                </div>

                <button
                    onClick={() => setShowMembers(true)}
                    className="text-sm bg-white/20 hover:bg-white/30 text-white px-3 py-1.5 rounded-lg transition-colors"
                >
                    👥 {members.length} member(s)
                </button>
            </div>

            {/* Kanban board */}
            <div className="px-6 pb-6 overflow-x-auto">
                <div className="flex gap-4 items-start min-w-max">
                    <ColumnList
                        columns={board.columns}
                        onAddTask={isMember ? createTask : async () => {}}
                        onDeleteTask={isMember ? deleteTask : async () => {}}
                        onDeleteColumn={isOwner ? deleteColumn : async () => {}}
                        onMoveTask={isMember ? handleMoveTask : async () => {}}
                        canEdit={isMember}
                    />
                    {isOwner && <CreateColumnForm onAdd={createColumn} />}
                </div>
            </div>

            {showMembers && (
                <MembersPanel
                    members={members}
                    isOwner={isOwner}
                    onInvite={handleInvite}
                    onRemove={handleRemoveMember}
                    onClose={() => setShowMembers(false)}
                />
            )}

            <ConnectionStatus />
        </div>
    );
}