import axios from 'axios';
import { toast } from 'react-toastify';

// API'nin temel URL'ini tanımlayın
const API_URL = import.meta.env.VITE_API_URL || 'https://localhost:7239/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// İstek interceptor'ı
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Yanıt interceptor'ı
api.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    
    // 401 hatası ve yeniden deneme yapmadıysa
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Refresh token isteği
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          // Refresh token yoksa, kullanıcıyı login'e yönlendir
          window.location.href = '/login';
          return Promise.reject(error);
        }

        const response = await axios.post(`${API_URL}/auth/refresh-token`, {
          refreshToken: refreshToken
        });

        if (response.data) {
          localStorage.setItem('accessToken', response.data.accessToken);
          localStorage.setItem('refreshToken', response.data.refreshToken);
          
          // Yeni token ile isteği tekrarla
          originalRequest.headers.Authorization = `Bearer ${response.data.accessToken}`;
          return axios(originalRequest);
        }
      } catch (refreshError) {
        // Refresh token hatası, kullanıcı tekrar giriş yapmalı
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        window.location.href = '/login';
        return Promise.reject(refreshError);
      }
    }

    // 400 hatası için mesajı göster
    if (error.response?.status === 400) {
      if (error.response.data && error.response.data.message) {
        toast.error(error.response.data.message);
      } else if (error.response.data && error.response.data.errors) {
        // Validation hatalarını göster
        const errorsObj = error.response.data.errors;
        const firstError = Object.values(errorsObj)[0];
        if (Array.isArray(firstError) && firstError.length > 0) {
          toast.error(firstError[0]);
        }
      }
    }

    // 500 hatası için genel mesaj
    if (error.response?.status === 500) {
      toast.error('Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.');
    }

    return Promise.reject(error);
  }
);

export default api; 