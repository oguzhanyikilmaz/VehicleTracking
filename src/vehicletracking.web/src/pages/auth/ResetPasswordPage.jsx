import { useState } from 'react';
import { useNavigate, useLocation, Link as RouterLink } from 'react-router-dom';
import { useFormik } from 'formik';
import * as Yup from 'yup';
import {
  TextField,
  Button,
  CircularProgress,
  Alert,
  Stack,
  Link,
  Box,
  Typography,
  InputAdornment,
  IconButton
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useAuth } from '../../contexts/AuthContext';

// Form doğrulama şeması
const resetPasswordSchema = Yup.object().shape({
  newPassword: Yup.string()
    .required('Yeni şifre gereklidir')
    .min(8, 'Şifre en az 8 karakter olmalıdır')
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
      'Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir'
    ),
  confirmPassword: Yup.string()
    .required('Şifreyi onaylayın')
    .oneOf([Yup.ref('newPassword')], 'Şifreler eşleşmiyor'),
});

const ResetPasswordPage = () => {
  const { resetPassword } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [error, setError] = useState(null);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  
  // URL'den userId ve token parametrelerini al
  const searchParams = new URLSearchParams(location.search);
  const userId = searchParams.get('userId');
  const token = searchParams.get('token');
  
  // Şifre göster/gizle
  const handleTogglePassword = () => {
    setShowPassword((prev) => !prev);
  };
  
  // Şifre onayı göster/gizle
  const handleToggleConfirmPassword = () => {
    setShowConfirmPassword((prev) => !prev);
  };
  
  // Form işlemleri
  const formik = useFormik({
    initialValues: {
      newPassword: '',
      confirmPassword: '',
    },
    validationSchema: resetPasswordSchema,
    onSubmit: async (values, { setSubmitting }) => {
      try {
        setError(null);
        
        if (!userId || !token) {
          setError('Geçersiz şifre sıfırlama bağlantısı');
          return;
        }
        
        const result = await resetPassword(
          userId,
          token,
          values.newPassword,
          values.confirmPassword
        );
        
        if (result) {
          navigate('/login');
        }
      } catch (err) {
        console.error('Şifre sıfırlama hatası:', err);
        setError(err.response?.data?.message || 'Şifre sıfırlama sırasında bir hata oluştu');
      } finally {
        setSubmitting(false);
      }
    }
  });
  
  // Geçersiz link kontrolü
  if (!userId || !token) {
    return (
      <Box sx={{ width: '100%', textAlign: 'center' }}>
        <Typography variant="h6" mb={3}>
          Geçersiz Şifre Sıfırlama Bağlantısı
        </Typography>
        
        <Alert severity="error" sx={{ mb: 3 }}>
          Bu bağlantı geçersiz veya süresi dolmuş. Lütfen yeni bir şifre sıfırlama isteği oluşturun.
        </Alert>
        
        <Button
          component={RouterLink}
          to="/forgot-password"
          variant="contained"
          fullWidth
          sx={{ mt: 2 }}
        >
          Şifremi Unuttum
        </Button>
        
        <Box sx={{ textAlign: 'center', mt: 2 }}>
          <Link
            component={RouterLink}
            to="/login"
            variant="body2"
            color="primary"
            underline="hover"
          >
            Giriş sayfasına dön
          </Link>
        </Box>
      </Box>
    );
  }
  
  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ width: '100%' }}>
      <Typography variant="h6" mb={3} textAlign="center">
        Yeni Şifre Belirleyin
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      <Stack spacing={3}>
        <TextField
          fullWidth
          id="newPassword"
          name="newPassword"
          label="Yeni Şifre"
          type={showPassword ? 'text' : 'password'}
          variant="outlined"
          value={formik.values.newPassword}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.newPassword && Boolean(formik.errors.newPassword)}
          helperText={formik.touched.newPassword && formik.errors.newPassword}
          autoComplete="new-password"
          disabled={formik.isSubmitting}
          autoFocus
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  aria-label="şifreyi göster/gizle"
                  onClick={handleTogglePassword}
                  edge="end"
                >
                  {showPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            )
          }}
        />
        
        <TextField
          fullWidth
          id="confirmPassword"
          name="confirmPassword"
          label="Yeni Şifre (Tekrar)"
          type={showConfirmPassword ? 'text' : 'password'}
          variant="outlined"
          value={formik.values.confirmPassword}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.confirmPassword && Boolean(formik.errors.confirmPassword)}
          helperText={formik.touched.confirmPassword && formik.errors.confirmPassword}
          autoComplete="new-password"
          disabled={formik.isSubmitting}
          InputProps={{
            endAdornment: (
              <InputAdornment position="end">
                <IconButton
                  aria-label="şifre onayını göster/gizle"
                  onClick={handleToggleConfirmPassword}
                  edge="end"
                >
                  {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                </IconButton>
              </InputAdornment>
            )
          }}
        />
        
        <Button
          type="submit"
          fullWidth
          variant="contained"
          size="large"
          disabled={formik.isSubmitting}
          sx={{
            mt: 2,
            py: 1.2,
          }}
        >
          {formik.isSubmitting ? (
            <CircularProgress size={24} color="inherit" />
          ) : (
            'Şifremi Sıfırla'
          )}
        </Button>
        
        <Box sx={{ textAlign: 'center', mt: 2 }}>
          <Link
            component={RouterLink}
            to="/login"
            variant="body2"
            color="primary"
            underline="hover"
          >
            Giriş sayfasına dön
          </Link>
        </Box>
      </Stack>
    </Box>
  );
};

export default ResetPasswordPage; 