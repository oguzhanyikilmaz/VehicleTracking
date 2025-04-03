import { createContext, useContext, useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import authService from '../services/authService';

// Auth context'ini oluştur
const AuthContext = createContext(null);

// Auth context hook'u
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

// Auth provider bileşeni
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [permissions, setPermissions] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Sayfa yüklendiğinde kullanıcı oturumunu kontrol et
  useEffect(() => {
    const checkAuth = async () => {
      try {
        if (authService.isAuthenticated()) {
          const currentUser = authService.getCurrentUser();
          if (currentUser) {
            setUser(currentUser);
            
            // Token'dan izinleri al
            const token = localStorage.getItem('accessToken');
            if (token) {
              try {
                const perms = authService.getPermissionsFromToken(token);
                setPermissions(perms);
              } catch (error) {
                console.error('İzinler alınırken hata oluştu:', error);
              }
            }
          }
        }
      } catch (error) {
        console.error('Kimlik doğrulama kontrolü sırasında hata:', error);
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, []);

  // Giriş fonksiyonu
  const login = async (username, password, rememberMe = false) => {
    try {
      setLoading(true);
      const result = await authService.login(username, password, rememberMe);
      
      if (result) {
        setUser(result.user);
        setPermissions(result.permissions || []);
        
        toast.success('Başarıyla giriş yapıldı');
        navigate('/dashboard');
        return true;
      }
      
      return false;
    } catch (error) {
      console.error('Giriş işlemi sırasında hata:', error);
      toast.error(error.response?.data?.message || 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Çıkış fonksiyonu
  const logout = async () => {
    try {
      setLoading(true);
      await authService.logout();
      
      // State'i temizle
      setUser(null);
      setPermissions([]);
      
      // Login sayfasına yönlendir
      navigate('/login');
      toast.info('Başarıyla çıkış yapıldı');
    } catch (error) {
      console.error('Çıkış işlemi sırasında hata:', error);
      toast.error('Çıkış yapılırken bir hata oluştu. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  // Kayıt fonksiyonu
  const register = async (registerData) => {
    try {
      setLoading(true);
      const result = await authService.register(registerData);
      
      if (result) {
        toast.success('Başarıyla kayıt oldunuz. Lütfen giriş yapın.');
        navigate('/login');
        return true;
      }
      
      return false;
    } catch (error) {
      console.error('Kayıt işlemi sırasında hata:', error);
      toast.error(error.response?.data?.message || 'Kayıt başarısız. Lütfen bilgilerinizi kontrol edin.');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Şifre değiştirme
  const changePassword = async (oldPassword, newPassword) => {
    try {
      setLoading(true);
      await authService.changePassword(oldPassword, newPassword);
      toast.success('Şifreniz başarıyla değiştirildi');
      return true;
    } catch (error) {
      console.error('Şifre değiştirme işlemi sırasında hata:', error);
      toast.error(error.response?.data?.message || 'Şifre değiştirilemedi. Lütfen tekrar deneyin.');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Şifremi unuttum
  const forgotPassword = async (email) => {
    try {
      setLoading(true);
      await authService.forgotPassword(email);
      toast.success('Şifre sıfırlama talimatları e-posta adresinize gönderildi');
      return true;
    } catch (error) {
      console.error('Şifremi unuttum işlemi sırasında hata:', error);
      toast.error(error.response?.data?.message || 'Şifremi unuttum işlemi başarısız. Lütfen tekrar deneyin.');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Şifre sıfırlama
  const resetPassword = async (userId, token, newPassword, confirmPassword) => {
    try {
      setLoading(true);
      await authService.resetPassword(userId, token, newPassword, confirmPassword);
      toast.success('Şifreniz başarıyla sıfırlandı. Lütfen yeni şifrenizle giriş yapın.');
      navigate('/login');
      return true;
    } catch (error) {
      console.error('Şifre sıfırlama işlemi sırasında hata:', error);
      toast.error(error.response?.data?.message || 'Şifre sıfırlama başarısız. Lütfen tekrar deneyin.');
      return false;
    } finally {
      setLoading(false);
    }
  };

  // Kullanıcının belirli bir izne sahip olup olmadığını kontrol et
  const hasPermission = (permission) => {
    return permissions.includes(permission);
  };

  // Context değeri
  const value = {
    user,
    permissions,
    loading,
    isAuthenticated: !!user,
    login,
    logout,
    register,
    changePassword,
    forgotPassword,
    resetPassword,
    hasPermission
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthContext; 