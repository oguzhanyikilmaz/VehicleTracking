import { Box, Container, Typography, Paper } from '@mui/material';
import { Outlet } from 'react-router-dom';
import vehicleTrackingLogo from '../assets/react.svg'; // Logo için placeholder kullanıyorum, gerçek logo ile değiştirilebilir

const AuthLayout = () => {
  // Şu anki yılı al
  const currentYear = new Date().getFullYear();

  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        minHeight: '100vh',
        backgroundColor: (theme) => theme.palette.background.default,
      }}
    >
      <Container component="main" maxWidth="xs" sx={{ mt: 8, mb: 4 }}>
        <Paper
          elevation={3}
          sx={{
            borderRadius: 2,
            px: 4,
            py: 3,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          {/* Logo */}
          <Box sx={{ mb: 2, display: 'flex', justifyContent: 'center' }}>
            <img src={vehicleTrackingLogo} alt="Vehicle Tracking" width="70" height="70" />
          </Box>
          
          {/* Uygulama adı */}
          <Typography component="h1" variant="h4" sx={{ mb: 4, fontWeight: 'bold' }}>
            Araç Takip Sistemi
          </Typography>
          
          {/* Çocuk bileşenler (Login, Register, vb.) */}
          <Outlet />
        </Paper>
      </Container>
      
      {/* Footer */}
      <Box
        component="footer"
        sx={{
          py: 3,
          mt: 'auto',
          textAlign: 'center',
          backgroundColor: 'transparent',
        }}
      >
        <Typography variant="body2" color="text.secondary">
          &copy; {currentYear} Vehicle Tracking. Tüm hakları saklıdır.
        </Typography>
      </Box>
    </Box>
  );
};

export default AuthLayout; 