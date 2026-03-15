import { useBoardStore } from '../store/boardStore';
import { useAuthStore } from '../store/authStore';

export function useRole() {
    const members = useBoardStore((s) => s.members);
    const user = useAuthStore((s) => s.user);

    const currentMember = members.find((m) => m.userId === user?.id);
    const role = currentMember?.role ?? null;

    return {
        role,
        isOwner: role === 'Owner',
        isMember: role === 'Member' || role === 'Owner',
        isViewer: role === 'Viewer',
    };
}