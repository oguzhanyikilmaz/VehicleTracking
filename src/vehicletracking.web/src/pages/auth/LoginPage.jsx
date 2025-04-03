import { useState } from 'react';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
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
  FormControlLabel,
  Checkbox,
  Typography,
  InputAdornment,
  IconButton
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useAuth } from '../../contexts/AuthContext';

// Form doğrulama şeması
const loginSchema = Yup.object().shape({
  username: Yup.string()
    .required('Kullanıcı adı gereklidir')
    .min(3, 'Kullanıcı adı en az 3 karakter olmalıdır'),
  password: Yup.string()
    .required('Şifre gereklidir')
    .min(6, 'Şifre en az 6 karakter olmalıdır'),
  rememberMe: Yup.boolean()
});

const LoginPage = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(null);
  const [showPassword, setShowPassword] = useState(false);
  
  // Şifre göster/gizle
  const handleTogglePassword = () => {
    setShowPassword((prev) => !prev);
  };
  
  // Form işlemleri
  const formik = useFormik({
    initialValues: {
      username: '',
      password: '',
      rememberMe: false
    },
    validationSchema: loginSchema,
    onSubmit: async (values, { setSubmitting }) => {
      try {
        setError(null);
        
        const success = await login(
          values.username,
          values.password,
          values.rememberMe
        );
        
        if (success) {
          navigate('/dashboard');
        }
      } catch (err) {
        console.error('Giriş hatası:', err);
        setError(err.response?.data?.message || 'Giriş yapılırken bir hata oluştu');
      } finally {
        setSubmitting(false);
      }
    }
  });
  
  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ width: '100%' }}>
      <Typography variant="h6" mb={3} textAlign="center">
        Hesabınıza Giriş Yapın
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      
      <Stack spacing={3}>
        <TextField
          fullWidth
          id="username"
          name="username"
          label="Kullanıcı Adı"
          variant="outlined"
          value={formik.values.username}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.username && Boolean(formik.errors.username)}
          helperText={formik.touched.username && formik.errors.username}
          autoComplete="username"
          disabled={formik.isSubmitting}
          autoFocus
        />
        
        <TextField
          fullWidth
          id="password"
          name="password"
          label="Şifre"
          type={showPassword ? 'text' : 'password'}
          variant="outlined"
          value={formik.values.password}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.password && Boolean(formik.errors.password)}
          helperText={formik.touched.password && formik.errors.password}
          autoComplete="current-password"
          disabled={formik.isSubmitting}
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
        
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <FormControlLabel
            control={
              <Checkbox
                id="rememberMe"
                name="rememberMe"
                checked={formik.values.rememberMe}
                onChange={formik.handleChange}
                color="primary"
                disabled={formik.isSubmitting}
              />
            }
            label="Beni hatırla"
          />
          
          <Link
            component={RouterLink}
            to="/forgot-password"
            variant="body2"
            color="primary"
            underline="hover"
          >
            Şifremi unuttum
          </Link>
        </Box>
        
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
            'Giriş Yap'
          )}
        </Button>
        
        <Box sx={{ textAlign: 'center', mt: 2 }}>
          <Typography variant="body2">
            Hesabınız yok mu?{' '}
            <Link
              component={RouterLink}
              to="/register"
              variant="body2"
              color="primary"
              underline="hover"
            >
              Kayıt olun
            </Link>
          </Typography>
        </Box>
      </Stack>
    </Box>
  );
};

export default LoginPage; 