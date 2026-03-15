import { useNavigate } from 'react-router-dom';
import type { BoardSummary } from '../../types';

interface Props {
    board: BoardSummary;
    onDelete: (id: string) => void;
}

export default function BoardCard({ board, onDelete }: Props) {
    const navigate = useNavigate();

    return (
        <div
            onClick={() => navigate(`/boards/${board.id}`)}
            className="bg-white rounded-xl shadow-sm border border-gray-200 p-5 cursor-pointer hover:shadow-md hover:border-blue-300 transition-all"
        >
            <div className="flex items-start justify-between">
                <h3 className="font-semibold text-gray-800 text-lg">{board.name}</h3>
                <button
                    onClick={(e) => {
                        e.stopPropagation(); // Không trigger navigate
                        onDelete(board.id);
                    }}
                    className="text-gray-400 hover:text-red-500 transition-colors text-sm"
                >
                    ✕
                </button>
            </div>
            <p className="text-sm text-gray-500 mt-2">Owner: {board.ownerName}</p>
            <p className="text-sm text-gray-400 mt-1">{board.memberCount} member(s)</p>
        </div>
    );
}