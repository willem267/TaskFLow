import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import BoardsPage from './pages/BoardsPage';
import BoardDetailPage from './pages/BoardDetailPage';
import ProtectedRoute from './components/ProtectedRoute';
import {useState, useEffect} from "react";
import {useAuthStore} from "./store/authStore.ts";




export default function App() {
        const [initializing, setInitializing] = useState(true);
        const refresh = useAuthStore((s) => s.refresh);
        useEffect(() => {
            refresh().finally(() => setInitializing(false));
        }, []);

    if (initializing) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50">
                <div className="text-gray-400 text-sm">Loading...</div>
            </div>
        );
    }
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route
                    path="/boards"
                    element={
                        <ProtectedRoute>
                            <BoardsPage />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/boards/:id"
                    element={
                        <ProtectedRoute>
                            <BoardDetailPage />
                        </ProtectedRoute>
                    }
                />
                <Route path="*" element={<Navigate to="/login" replace />} />
            </Routes>
        </BrowserRouter>
    );
}