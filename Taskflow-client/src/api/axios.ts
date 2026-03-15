import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7000/api',
    withCredentials: true, // Gửi cookie refresh token tự động
});

// Lưu access token trong memory
let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => {
    accessToken = token;
};

export const getAccessToken = () => accessToken;


api.interceptors.request.use((config) => {
    if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
});


api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const original = error.config;

        if (error.response?.status === 401 && !original._retry) {
            original._retry = true;

            try {
                const { data } = await axios.post(
                    'https://localhost:7000/api/auth/refresh',
                    {},
                    { withCredentials: true }
                );

                setAccessToken(data.accessToken);
                original.headers.Authorization = `Bearer ${data.accessToken}`;

                return api(original); // Retry request gốc
            } catch {
                setAccessToken(null);
                window.location.href = '/login';
            }
        }

        return Promise.reject(error);
    }
);

export default api;