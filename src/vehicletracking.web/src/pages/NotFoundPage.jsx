import { useNavigate } from 'react-router-dom';
import { Box, Button, Typography, Container } from '@mui/material';
import { SentimentDissatisfied as ErrorIcon } from '@mui/icons-material';

const NotFoundPage = () => {
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
        <ErrorIcon sx={{ fontSize: 100, color: 'text.secondary', mb: 4 }} />
        
        <Typography variant="h2" component="h1" gutterBottom color="primary" fontWeight="bold">
          404
        </Typography>
        
        <Typography variant="h4" gutterBottom>
          Sayfa Bulunamadı
        </Typography>
        
        <Typography variant="body1" color="text.secondary" paragraph sx={{ mb: 4 }}>
          Aradığınız sayfa mevcut değil veya taşınmış olabilir.
        </Typography>
        
        <Button
          variant="contained"
          size="large"
          onClick={() => navigate('/')}
          sx={{ mt: 2 }}
        >
          Ana Sayfaya Dön
        </Button>
      </Box>
    </Container>
  );
};

export default NotFoundPage;
