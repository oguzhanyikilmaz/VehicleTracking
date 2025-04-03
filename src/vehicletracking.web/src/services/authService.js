import api from './api';
import jwtDecode from 'jwt-decode';

const USER_KEY = 'user';
const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';

// Kullanıcı bilgilerini localStorage'dan alır
const getUser = () => {
  const userStr = localStorage.getItem(USER_KEY);
  if (userStr) {
    try {
      return JSON.parse(userStr);
    } catch (e) {
      console.error('User bilgisi parse edilemedi:', e);
      return null;
    }
  }
  return null;
};

// Token'dan yetkileri çıkartır
const getPermissionsFromToken = (token) => {
  if (!token) return [];
  try {
    const decoded = jwtDecode(token);
    return decoded.permissions || [];
  } catch (error) {
    console.error('Token çözümlenirken hata oluştu:', error);
    return [];
  }
};

const authService = {
  // Kullanıcı girişi
  login: async (username, password, rememberMe = false) => {
    const response = await api.post('/auth/login', {
      username,
      password,
      rememberMe
    });
    
    if (response.data) {
      const { accessToken, refreshToken, user } = response.data;
      localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
      localStorage.setItem(USER_KEY, JSON.stringify(user));
      
      return {
        user,
        permissions: getPermissionsFromToken(accessToken)
      };
    }
    
    return null;
  },
  
  // Kullanıcı çıkışı
  logout: async () => {
    try {
      // API'ye çıkış isteği gönder
      await api.post('/auth/logout');
    } catch (error) {
      console.error('Çıkış yapılırken hata oluştu:', error);
    } finally {
      // Yerel depolamadan kimlik bilgilerini temizle
      localStorage.removeItem(ACCESS_TOKEN_KEY);
      localStorage.removeItem(REFRESH_TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
    }
  },
  
  // Kullanıcı kaydı
  register: async (registerData) => {
    const response = await api.post('/auth/register', registerData);
    return response.data;
  },
  
  // Mevcut kullanıcıyı al
  getCurrentUser: () => {
    return getUser();
  },
  
  // Token'ı yenile
  refreshToken: async () => {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!refreshToken) {
      return null;
    }
    
    const response = await api.post('/auth/refresh-token', {
      refreshToken
    });
    
    if (response.data) {
      const { accessToken, refreshToken: newRefreshToken } = response.data;
      localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
      localStorage.setItem(REFRESH_TOKEN_KEY, newRefreshToken);
      
      return {
        accessToken,
        refreshToken: newRefreshToken,
        permissions: getPermissionsFromToken(accessToken)
      };
    }
    
    return null;
  },
  
  // Parola değiştirme
  changePassword: async (oldPassword, newPassword) => {
    const response = await api.post('/auth/change-password', {
      oldPassword,
      newPassword
    });
    return response.data;
  },
  
  // Şifremi unuttum
  forgotPassword: async (email) => {
    const response = await api.post('/auth/forgot-password', { email });
    return response.data;
  },
  
  // Parola sıfırlama
  resetPassword: async (userId, token, newPassword, confirmPassword) => {
    const response = await api.post('/auth/reset-password', {
      userId,
      token,
      newPassword,
      confirmPassword
    });
    return response.data;
  },
  
  // Kullanıcının giriş yapmış olup olmadığını kontrol et
  isAuthenticated: () => {
    const token = localStorage.getItem(ACCESS_TOKEN_KEY);
    if (!token) return false;
    
    try {
      const decoded = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      
      // Token süresi dolmuşsa false dön
      return decoded.exp > currentTime;
    } catch (error) {
      return false;
    }
  },
  
  // Kullanıcının belirli bir yetkisi olup olmadığını kontrol et
  hasPermission: (permission) => {
    const token = localStorage.getItem(ACCESS_TOKEN_KEY);
    const permissions = getPermissionsFromToken(token);
    return permissions.includes(permission);
  },
  
  // Token'dan izinleri çıkartır (AuthContext için export)
  getPermissionsFromToken
};

export default authService; 