import { useState } from 'react';

interface Props {
    onAdd: (title: string) => Promise<void>;
    onCancel: () => void;
}

export default function CreateTaskForm({ onAdd, onCancel }: Props) {
    const [title, setTitle] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!title.trim()) return;

        setLoading(true);
        try {
            await onAdd(title.trim());
            setTitle('');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="mt-2">
      <textarea
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Enter task title..."
          autoFocus
          rows={2}
          className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm resize-none focus:outline-none focus:ring-2 focus:ring-blue-500"
      />
            <div className="flex gap-2 mt-2">
                <button
                    type="submit"
                    disabled={loading || !title.trim()}
                    className="px-3 py-1.5 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                >
                    {loading ? 'Adding...' : 'Add'}
                </button>
                <button
                    type="button"
                    onClick={onCancel}
                    className="px-3 py-1.5 text-sm text-gray-500 hover:bg-gray-100 rounded-lg"
                >
                    Cancel
                </button>
            </div>
        </form>
    );
}