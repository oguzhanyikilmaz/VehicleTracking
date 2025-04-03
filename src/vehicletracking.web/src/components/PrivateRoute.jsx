import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { CircularProgress, Box } from '@mui/material';

/**
 * PrivateRoute bileşeni, kimlik doğrulaması gerektiren sayfaları korumak için kullanılır.
 * Kullanıcı girişi yapılmadıysa login sayfasına yönlendirir.
 */
const PrivateRoute = ({ requiredPermission = null }) => {
  const { isAuthenticated, hasPermission, loading } = useAuth();
  const location = useLocation();
  
  // Kimlik doğrulama işlemi devam ediyorsa loading göster
  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }
  
  // Kullanıcı giriş yapmadıysa login sayfasına yönlendir
  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }
  
  // Belirli bir yetki gerekiyorsa ve kullanıcıda bu yetki yoksa 403 sayfasına yönlendir
  if (requiredPermission && !hasPermission(requiredPermission)) {
    return <Navigate to="/forbidden" replace />;
  }
  
  // Kullanıcı giriş yapmış ve gerekli yetkiye sahipse içeriği göster
  return <Outlet />;
};

export default PrivateRoute; 