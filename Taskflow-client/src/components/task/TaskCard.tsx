import { useState } from 'react';
import { Draggable } from '@hello-pangea/dnd';
import type { TaskItem, Member } from '../../types';
import TaskDetail from './TaskDetail';

interface Props {
    task: TaskItem;
    index: number;
    members: Member[];
    onDelete: (taskId: string) => void;
    onUpdate: (taskId: string, payload: any) => Promise<void>;
}

export default function TaskCard({ task, index, members, onDelete, onUpdate }: Props) {
    const [showDetail, setShowDetail] = useState(false);

    return (
        <>
            <Draggable draggableId={task.id} index={index}>
                {(provided, snapshot) => (
                    <div
                        ref={provided.innerRef}
                        {...provided.draggableProps}
                        {...provided.dragHandleProps}
                        onClick={() => setShowDetail(true)}
                        className={`bg-white rounded-lg border border-gray-200 p-3 mb-2 cursor-pointer active:cursor-grabbing transition-shadow ${
                            snapshot.isDragging ? 'shadow-lg rotate-1' : 'hover:shadow-sm'
                        }`}
                    >
                        <div className="flex items-start justify-between gap-2">
                            <p className="text-sm text-gray-800 font-medium">{task.title}</p>
                            <button
                                onClick={(e) => {
                                    e.stopPropagation(); // Không trigger setShowDetail
                                    onDelete(task.id);
                                }}
                                className="text-gray-300 hover:text-red-400 text-xs shrink-0"
                            >
                                ✕
                            </button>
                        </div>

                        {task.description && (
                            <p className="text-xs text-gray-400 mt-1 line-clamp-2">
                                {task.description}
                            </p>
                        )}

                        {task.dueDate && (
                            <p className="text-xs text-orange-400 mt-2">
                                📅 {new Date(task.dueDate).toLocaleDateString()}
                            </p>
                        )}

                        {task.assignee && (
                            <p className="text-xs text-gray-400 mt-1">
                                👤 {task.assignee.name}
                            </p>
                        )}
                    </div>
                )}
            </Draggable>

            {showDetail && (
                <TaskDetail
                    task={task}
                    members={members}
                    onUpdate={onUpdate}
                    onClose={() => setShowDetail(false)}
                />
            )}
        </>
    );
}