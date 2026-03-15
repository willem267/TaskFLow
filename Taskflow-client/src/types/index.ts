export interface User {
    id: string;
    email: string;
    name: string;
    avatar?: string;
}

export interface AuthResponse {
    accessToken: string;
    accessTokenExpiresAt: string;
    user: User;
}

export interface TaskItem {
    id: string;
    title: string;
    description?: string;
    position: number;
    dueDate?: string;
    createdAt: string;
    assignee?: {
        id: string;
        name: string;
        avatar?: string;
    };
}

export interface Column {
    id: string;
    name: string;
    position: number;
    tasks: TaskItem[];
}

export interface BoardDetail {
    id: string;
    name: string;
    createdAt: string;
    columns: Column[];
}

export interface BoardSummary {
    id: string;
    name: string;
    ownerName: string;
    memberCount: number;
    createdAt: string;
}

export interface TaskItem {
    id: string;
    columnId?: string;
    title: string;
    description?: string;
    position: number;
    dueDate?: string;
    createdAt: string;
    assignee?: {
        id: string;
        name: string;
        avatar?: string;
    };
}


export interface Member {
    userId: string;
    name: string;
    email: string;
    role: 'Owner' | 'Member' | 'Viewer';
}

export interface BoardDetail {
    id: string;
    name: string;
    createdAt: string;
    columns: Column[];
    members?: Member[];
}