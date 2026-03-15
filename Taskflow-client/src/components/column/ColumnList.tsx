import { DragDropContext, type DropResult } from '@hello-pangea/dnd';
import type {Column, Member} from '../../types';
import ColumnCard from './ColumnCard';


interface Props {
    columns: Column[];
    members: Member[];
    onAddTask: (columnId: string, title: string) => Promise<void>;
    onDeleteTask: (taskId: string) => void;
    onUpdateTask: (taskId: string, payload: any) => Promise<void>;
    onDeleteColumn: (columnId: string) => void;
    onMoveTask: (
        taskId: string,
        targetColumnId: string,
        previousTaskId: string | null,
        nextTaskId: string | null,
        newPosition: number
    ) => void;
    canEdit: boolean;
}


export default function ColumnList({
                                       columns,
                                       onAddTask,
                                       onDeleteTask,
                                       onUpdateTask,
                                       onDeleteColumn,
                                       onMoveTask,
                                       canEdit,
                                   }: Props) {
    const handleDragEnd = (result: DropResult) => {
        const { draggableId, source, destination } = result;


        if (!destination) return;

        if (
            source.droppableId === destination.droppableId &&
            source.index === destination.index
        ) return;

        const targetColumn = columns.find((c) => c.id === destination.droppableId);
        if (!targetColumn) return;

        const targetTasks = targetColumn.tasks.filter((t) => t.id !== draggableId);

        const previousTask = targetTasks[destination.index - 1] ?? null;
        const nextTask = targetTasks[destination.index] ?? null;

        // Tính position mới
        const prevPosition = previousTask?.position ?? 0;
        const nextPosition = nextTask?.position ?? prevPosition + 2000;
        const newPosition = (prevPosition + nextPosition) / 2;

        onMoveTask(
            draggableId,
            destination.droppableId,
            previousTask?.id ?? null,
            nextTask?.id ?? null,
            newPosition
        );
    };

    return (
        <DragDropContext onDragEnd={handleDragEnd}>
            <div className="flex gap-4 items-start">
                {columns
                    .sort((a, b) => a.position - b.position)
                    .map((column) => (
                        <ColumnCard
                            key={column.id}
                            column={column}
                            onAddTask={onAddTask}
                            onDeleteTask={onDeleteTask}
                            onUpdateTask={onUpdateTask}
                            onDeleteColumn={onDeleteColumn}
                            canEdit={canEdit}
                        />
                    ))}
            </div>
        </DragDropContext>
    );
}