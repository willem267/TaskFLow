import { useState } from 'react';

interface Props {
    onAdd: (name: string) => Promise<void>;
}

export default function CreateColumnForm({ onAdd }: Props) {
    const [show, setShow] = useState(false);
    const [name, setName] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!name.trim()) return;

        setLoading(true);
        try {
            await onAdd(name.trim());
            setName('');
            setShow(false);
        } finally {
            setLoading(false);
        }
    };

    if (!show) {
        return (
            <button
                onClick={() => setShow(true)}
                className="w-72 shrink-0 bg-white/70 hover:bg-white border border-dashed border-gray-300 rounded-xl p-3 text-sm text-gray-500 hover:text-gray-700 transition-all"
            >
                + Add column
            </button>
        );
    }

    return (
        <form
            onSubmit={handleSubmit}
            className="w-72 shrink-0 bg-gray-100 rounded-xl p-3"
        >
            <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                placeholder="Column name..."
                autoFocus
                className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <div className="flex gap-2 mt-2">
                <button
                    type="submit"
                    disabled={loading || !name.trim()}
                    className="px-3 py-1.5 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                    {loading ? 'Adding...' : 'Add'}
                </button>
                <button
                    type="button"
                    onClick={() => setShow(false)}
                    className="px-3 py-1.5 text-sm text-gray-500 hover:bg-gray-200 rounded-lg"
                >
                    Cancel
                </button>
            </div>
        </form>
    );
}