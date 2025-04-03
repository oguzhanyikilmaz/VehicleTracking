import { useNavigate } from 'react-router-dom';
import { Box, Button, Typography, Container } from '@mui/material';
import { Block as BlockIcon } from '@mui/icons-material';

const ForbiddenPage = () => {
  const navigate = useNavigate();

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          textAlign: 'center',
          py: 5,
        }}
      >
        <BlockIcon sx={{ fontSize: 100, color: 'error.main', mb: 4 }} />
        
        <Typography variant="h2" component="h1" gutterBottom color="error" fontWeight="bold">
          403
        </Typography>
        
        <Typography variant="h4" gutterBottom>
          Erişim Engellendi
        </Typography>
        
        <Typography variant="body1" color="text.secondary" paragraph sx={{ mb: 4 }}>
          Bu sayfaya erişim yetkiniz bulunmamaktadır.
        </Typography>
        
        <Button
          variant="contained"
          size="large"
          onClick={() => navigate('/dashboard')}
          sx={{ mt: 2 }}
        >
          Dashboard'a Dön
        </Button>
      </Box>
    </Container>
  );
};

export default ForbiddenPage; 