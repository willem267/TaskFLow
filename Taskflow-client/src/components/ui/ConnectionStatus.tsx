import { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';
import { getConnection } from '../../utils/signalr';

export default function ConnectionStatus() {
    const [status, setStatus] = useState<signalR.HubConnectionState>(
        signalR.HubConnectionState.Disconnected
    );

    useEffect(() => {
        const interval = setInterval(() => {
            const conn = getConnection();
            if (conn) setStatus(conn.state);
        }, 1000);

        return () => clearInterval(interval);
    }, []);

    if (status === signalR.HubConnectionState.Connected) return null;

    return (
        <div className="fixed bottom-4 right-4 bg-yellow-500 text-white text-xs px-3 py-2 rounded-full shadow">
            {status === signalR.HubConnectionState.Reconnecting
                ? '🔄 Reconnecting...'
                : '⚠️ Disconnected'}
        </div>
    );
}