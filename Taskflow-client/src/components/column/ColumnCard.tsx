import { useState } from 'react';
import { Droppable } from '@hello-pangea/dnd';
import type {Column, Member} from '../../types';
import TaskCard from '../task/TaskCard';
import CreateTaskForm from '../task/CreateTaskForm';

interface Props {
    column: Column;
    members: Member[];
    onAddTask: (columnId: string, title: string) => Promise<void>;
    onDeleteTask: (taskId: string) => void;
    onUpdateTask: (taskId: string, payload: any) => Promise<void>;
    onDeleteColumn: (columnId: string) => void;
    canEdit: boolean;
}

export default function ColumnCard({
                                       column,
                                       onAddTask,
                                       onDeleteTask,
                                       onDeleteColumn,
                                        onUpdateTask,
                                       canEdit,
                                   }: Props) {
    const [showForm, setShowForm] = useState(false);

    return (
        <div className="bg-gray-100 rounded-xl p-3 w-72 shrink-0 flex flex-col">
            {/* Column header */}
            <div className="flex items-center justify-between mb-3">
                <h3 className="font-semibold text-gray-700 text-sm">
                    {column.name}
                    <span className="ml-2 text-gray-400 font-normal">
            {column.tasks.length}
          </span>
                </h3>
                <button
                    onClick={() => onDeleteColumn(column.id)}
                    className="text-gray-300 hover:text-red-400 text-sm"
                >
                    ✕
                </button>
            </div>

            {/* Tasks drop zone */}
            <Droppable droppableId={column.id}>
                {(provided, snapshot) => (
                    <div
                        ref={provided.innerRef}
                        {...provided.droppableProps}
                        className={`flex-1 min-h-16 rounded-lg transition-colors ${
                            snapshot.isDraggingOver ? 'bg-blue-50' : ''
                        }`}
                    >
                        {column.tasks.map((task, index) => (
                            <TaskCard
                                key={task.id}
                                task={task}
                                index={index}
                                onDelete={onDeleteTask}
                                onUpdate={onUpdateTask}
                            />
                        ))}
                        {provided.placeholder}
                    </div>
                )}
            </Droppable>
            {canEdit && (
                <div className="mt-2">
                    {showForm ? (
                        <CreateTaskForm
                            onAdd={async (title) => {
                                await onAddTask(column.id, title);
                                setShowForm(false);
                            }}
                            onCancel={() => setShowForm(false)}
                        />
                    ) : (
                        <button
                            onClick={() => setShowForm(true)}
                            className="w-full text-sm text-gray-500 hover:text-gray-700 hover:bg-gray-200 rounded-lg py-1.5 px-2 text-left transition-colors"
                        >
                            + Add task
                        </button>
                    )}
                </div>
            )}
        </div>
    );
}