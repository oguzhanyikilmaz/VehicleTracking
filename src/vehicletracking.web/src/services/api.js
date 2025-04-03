import axios from 'axios';
import { toast } from 'react-toastify';

// API bağlantı hatası mesajları
const CONNECTION_ERROR_MSG = 'Sunucuya bağlanılamadı. İnternet bağlantınızı kontrol edin.';
const TIMEOUT_ERROR_MSG = 'İstek zaman aşımına uğradı. Lütfen daha sonra tekrar deneyin.';

// API'nin temel URL'ini tanımlayın
const API_URL = '/api';

console.log('API URL:', API_URL); // Geliştirme sırasında API URL'i görmek için

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 15000, // 15 saniye
});

// İstek interceptor'ı
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Debug amaçlı
    console.log(`${config.method.toUpperCase()} ${config.baseURL}${config.url}`);
    
    return config;
  },
  (error) => {
    console.error('İstek hatası:', error);
    return Promise.reject(error);
  }
);

// Yanıt interceptor'ı
api.interceptors.response.use(
  (response) => {
    // Debug amaçlı
    console.log('API Yanıtı:', response.status, response.data);
    return response;
  },
  async (error) => {
    console.error('API Hatası:', error);
    
    // Ağ hatası veya sunucu çalışmıyor
    if (!error.response) {
      if (error.code === 'ECONNABORTED') {
        toast.error(TIMEOUT_ERROR_MSG);
      } else {
        toast.error(CONNECTION_ERROR_MSG);
      }
      return Promise.reject(error);
    }
    
    const originalRequest = error.config;
    
    // 401 hatası ve yeniden deneme yapmadıysa
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        // Refresh token isteği
        const refreshToken = localStorage.getItem('refreshToken');
        if (!refreshToken) {
          // Refresh token yoksa, kullanıcıyı login'e yönlendir
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');
          window.location.href = '/login';
          return Promise.reject(error);
        }

        console.log('Token yenileme isteği gönderiliyor...');
        const response = await axios.post(`${API_URL}/auth/refresh-token`, {
          refreshToken: refreshToken
        });

        if (response.data) {
          console.log('Token yenilendi');
          localStorage.setItem('accessToken', response.data.accessToken);
          localStorage.setItem('refreshToken', response.data.refreshToken);
          
          // Yeni token ile isteği tekrarla
          originalRequest.headers.Authorization = `Bearer ${response.data.accessToken}`;
          return axios(originalRequest);
        }
      } catch (refreshError) {
        console.error('Token yenileme hatası:', refreshError);
        // Refresh token hatası, kullanıcı tekrar giriş yapmalı
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        toast.error('Oturumunuz sona erdi. Lütfen tekrar giriş yapın.');
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
        } else {
          // Validation nesnesi başka bir formatta olabilir
          toast.error('Girilen bilgilerde hatalar var. Lütfen kontrol ediniz.');
        }
      } else {
        toast.error('İstek işlenirken bir hata oluştu.');
      }
    }

    // 404 hatası için özel mesaj
    if (error.response?.status === 404) {
      toast.error('İstenen kaynak bulunamadı.');
    }

    // 500 hatası için genel mesaj
    if (error.response?.status === 500) {
      toast.error('Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.');
    }

    return Promise.reject(error);
  }
);

export default api; 