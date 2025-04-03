import { useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
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
} from '@mui/material';
import { useAuth } from '../../contexts/AuthContext';

// Form doğrulama şeması
const forgotPasswordSchema = Yup.object().shape({
  email: Yup.string()
    .required('E-posta gereklidir')
    .email('Geçerli bir e-posta adresi girin'),
});

const ForgotPasswordPage = () => {
  const { forgotPassword } = useAuth();
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  
  // Form işlemleri
  const formik = useFormik({
    initialValues: {
      email: '',
    },
    validationSchema: forgotPasswordSchema,
    onSubmit: async (values, { setSubmitting }) => {
      try {
        setError(null);
        setSuccess(false);
        
        const result = await forgotPassword(values.email);
        
        if (result) {
          setSuccess(true);
        }
      } catch (err) {
        console.error('Şifremi unuttum hatası:', err);
        setError(err.response?.data?.message || 'İşlem sırasında bir hata oluştu');
      } finally {
        setSubmitting(false);
      }
    }
  });
  
  return (
    <Box component="form" onSubmit={formik.handleSubmit} sx={{ width: '100%' }}>
      <Typography variant="h6" mb={3} textAlign="center">
        Şifrenizi mi Unuttunuz?
      </Typography>
      
      <Typography variant="body2" mb={3} textAlign="center" color="text.secondary">
        E-posta adresinizi girin. Size şifrenizi sıfırlamanız için bir bağlantı göndereceğiz.
      </Typography>
      
      {error && (
        <Alert severity="error" sx={{ mb: 3 }}>
          {error}
        </Alert>
      )}
      
      {success && (
        <Alert severity="success" sx={{ mb: 3 }}>
          Şifre sıfırlama talimatları e-posta adresinize gönderildi. Lütfen e-postanızı kontrol edin.
        </Alert>
      )}
      
      <Stack spacing={3}>
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
          disabled={formik.isSubmitting || success}
          autoFocus
        />
        
        <Button
          type="submit"
          fullWidth
          variant="contained"
          size="large"
          disabled={formik.isSubmitting || success}
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

export default ForgotPasswordPage; 