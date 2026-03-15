import * as signalR from '@microsoft/signalr';
import { getAccessToken } from '../api/axios';

let connection: signalR.HubConnection | null = null;

export const createConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl('https://localhost:7000/hubs/board', {
            accessTokenFactory: () => getAccessToken() ?? '',
        })
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Warning)
        .build();

    return connection;
};

export const getConnection = () => connection;

export const startConnection = async () => {
    if (!connection) createConnection();

    if (connection!.state === signalR.HubConnectionState.Disconnected) {
        await connection!.start();
    }
};

export const stopConnection = async () => {
    if (connection?.state === signalR.HubConnectionState.Connected) {
        await connection.stop();
    }
};

export const joinBoard = async (boardId: string) => {
    await connection?.invoke('JoinBoard', boardId);
};

export const leaveBoard = async (boardId: string) => {
    await connection?.invoke('LeaveBoard', boardId);
};