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
  Typography,
  InputAdornment,
  IconButton,
  Grid,
} from '@mui/material';
import { Visibility, VisibilityOff } from '@mui/icons-material';
import { useAuth } from '../../contexts/AuthContext';

// Form doğrulama şeması
const registerSchema = Yup.object().shape({
  firstName: Yup.string()
    .required('Ad gereklidir')
    .min(2, 'Ad en az 2 karakter olmalıdır'),
  lastName: Yup.string()
    .required('Soyad gereklidir')
    .min(2, 'Soyad en az 2 karakter olmalıdır'),
  username: Yup.string()
    .required('Kullanıcı adı gereklidir')
    .min(3, 'Kullanıcı adı en az 3 karakter olmalıdır')
    .matches(/^[a-zA-Z0-9._-]+$/, 'Kullanıcı adı yalnızca harf, rakam ve ._- karakterleri içerebilir'),
  email: Yup.string()
    .required('E-posta gereklidir')
    .email('Geçerli bir e-posta adresi girin'),
  phoneNumber: Yup.string()
    .matches(/^[0-9+\-\s]+$/, 'Geçerli bir telefon numarası girin'),
  password: Yup.string()
    .required('Şifre gereklidir')
    .min(8, 'Şifre en az 8 karakter olmalıdır')
    .matches(
      /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/,
      'Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir'
    ),
  confirmPassword: Yup.string()
    .required('Şifreyi onaylayın')
    .oneOf([Yup.ref('password')], 'Şifreler eşleşmiyor'),
});

const RegisterPage = () => {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState(null);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  
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
      firstName: '',
      lastName: '',
      username: '',
      email: '',
      phoneNumber: '',
      password: '',
      confirmPassword: '',
    },
    validationSchema: registerSchema,
    onSubmit: async (values, { setSubmitting }) => {
      try {
        setError(null);
        
        const registerData = {
          firstName: values.firstName,
          lastName: values.lastName,
          username: values.username,
          email: values.email,
          phoneNumber: values.phoneNumber,
          password: values.password,
          confirmPassword: values.confirmPassword,
        };
        
        const success = await register(registerData);
        
        if (success) {
          navigate('/login');
        }
      } catch (err) {
        console.error('Kayıt hatası:', err);
        setError(err.response?.data?.message || 'Kayıt yapılırken bir hata oluştu');
      } finally {
        setSubmitting(false);
      }
    }
  });
  
  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ width: '100%' }}>
      <Typography variant="h6" mb={3} textAlign="center">
        Yeni Hesap Oluştur
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      
      <Stack spacing={3}>
        <Grid container spacing={2}>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="firstName"
              name="firstName"
              label="Ad"
              variant="outlined"
              value={formik.values.firstName}
              onChange={formik.handleChange}
              onBlur={formik.handleBlur}
              error={formik.touched.firstName && Boolean(formik.errors.firstName)}
              helperText={formik.touched.firstName && formik.errors.firstName}
              autoComplete="given-name"
              disabled={formik.isSubmitting}
              autoFocus
            />
          </Grid>
          <Grid item xs={12} sm={6}>
            <TextField
              fullWidth
              id="lastName"
              name="lastName"
              label="Soyad"
              variant="outlined"
              value={formik.values.lastName}
              onChange={formik.handleChange}
              onBlur={formik.handleBlur}
              error={formik.touched.lastName && Boolean(formik.errors.lastName)}
              helperText={formik.touched.lastName && formik.errors.lastName}
              autoComplete="family-name"
              disabled={formik.isSubmitting}
            />
          </Grid>
        </Grid>
        
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
        />
        
        <TextField
          fullWidth
          id="email"
          name="email"
          label="E-posta"
          variant="outlined"
          type="email"
          value={formik.values.email}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.email && Boolean(formik.errors.email)}
          helperText={formik.touched.email && formik.errors.email}
          autoComplete="email"
          disabled={formik.isSubmitting}
        />
        
        <TextField
          fullWidth
          id="phoneNumber"
          name="phoneNumber"
          label="Telefon Numarası (Opsiyonel)"
          variant="outlined"
          value={formik.values.phoneNumber}
          onChange={formik.handleChange}
          onBlur={formik.handleBlur}
          error={formik.touched.phoneNumber && Boolean(formik.errors.phoneNumber)}
          helperText={formik.touched.phoneNumber && formik.errors.phoneNumber}
          autoComplete="tel"
          disabled={formik.isSubmitting}
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
          autoComplete="new-password"
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
        
        <TextField
          fullWidth
          id="confirmPassword"
          name="confirmPassword"
          label="Şifre (Tekrar)"
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
            'Kayıt Ol'
          )}
        </Button>
        
        <Box sx={{ textAlign: 'center', mt: 2 }}>
          <Typography variant="body2">
            Zaten bir hesabınız var mı?{' '}
            <Link
              component={RouterLink}
              to="/login"
              variant="body2"
              color="primary"
              underline="hover"
            >
              Giriş yapın
            </Link>
          </Typography>
        </Box>
      </Stack>
    </Box>
  );
};

export default RegisterPage; 