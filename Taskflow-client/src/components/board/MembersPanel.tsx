import { useState } from 'react';
import type { Member } from '../../types';

interface Props {
    members: Member[];
    isOwner: boolean;
    onInvite: (email: string) => Promise<void>;
    onRemove: (userId: string) => Promise<void>;
    onClose: () => void;
}

export default function MembersPanel({
                                         members,
                                         isOwner,
                                         onInvite,
                                         onRemove,
                                         onClose,
                                     }: Props) {
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleInvite = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!email.trim()) return;

        setLoading(true);
        setError('');
        try {
            await onInvite(email.trim());
            setEmail('');
        } catch (err: any) {
            setError(err?.response?.data?.error ?? 'Failed to invite user.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl shadow-lg w-full max-w-md p-6">
                <div className="flex items-center justify-between mb-4">
                    <h2 className="text-lg font-semibold text-gray-800">Members</h2>
                    <button
                        onClick={onClose}
                        className="text-gray-400 hover:text-gray-600"
                    >
                        ✕
                    </button>
                </div>

                {/* Member list */}
                <div className="space-y-2 mb-6 max-h-64 overflow-y-auto">
                    {members.map((member) => (
                        <div
                            key={member.userId}
                            className="flex items-center justify-between py-2 px-3 bg-gray-50 rounded-lg"
                        >
                            <div>
                                <p className="text-sm font-medium text-gray-800">{member.name}</p>
                                <p className="text-xs text-gray-400">{member.email}</p>
                            </div>
                            <div className="flex items-center gap-2">
                <span
                    className={`text-xs px-2 py-0.5 rounded-full ${
                        member.role === 'Owner'
                            ? 'bg-blue-100 text-blue-600'
                            : 'bg-gray-100 text-gray-500'
                    }`}
                >
                  {member.role}
                </span>
                                {isOwner && member.role !== 'Owner' && (
                                    <button
                                        onClick={() => onRemove(member.userId)}
                                        className="text-gray-300 hover:text-red-400 text-xs"
                                    >
                                        ✕
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>

                {/* Invite form — chỉ Owner thấy */}
                {isOwner && (
                    <form onSubmit={handleInvite}>
                        <p className="text-sm font-medium text-gray-700 mb-2">
                            Invite by email
                        </p>
                        {error && (
                            <p className="text-xs text-red-500 mb-2">{error}</p>
                        )}
                        <div className="flex gap-2">
                            <input
                                type="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="email@example.com"
                                className="flex-1 border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                            />
                            <button
                                type="submit"
                                disabled={loading || !email.trim()}
                                className="px-4 py-2 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                            >
                                {loading ? '...' : 'Invite'}
                            </button>
                        </div>
                    </form>
                )}
            </div>
        </div>
    );
}