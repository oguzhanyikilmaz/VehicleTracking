import { useState } from 'react';
import { useNavigate, Outlet } from 'react-router-dom';
import {
  AppBar,
  Box,
  Toolbar,
  IconButton,
  Typography,
  Menu,
  MenuItem,
  Drawer,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Divider,
  Avatar,
  useMediaQuery,
  Tooltip
} from '@mui/material';
import {
  Menu as MenuIcon,
  ChevronLeft as ChevronLeftIcon,
  Dashboard as DashboardIcon,
  DirectionsCar as DirectionsCarIcon,
  Router as RouterIcon,
  People as PeopleIcon,
  Settings as SettingsIcon,
  Logout as LogoutIcon,
  AccountCircle as AccountCircleIcon,
  Business as BusinessIcon
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';

// Drawer genişliği
const drawerWidth = 240;

const MainLayout = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const isMobile = useMediaQuery((theme) => theme.breakpoints.down('md'));
  
  // Drawer açık/kapalı durumu
  const [drawerOpen, setDrawerOpen] = useState(!isMobile);
  
  // Kullanıcı menüsü açık/kapalı durumu
  const [anchorEl, setAnchorEl] = useState(null);
  const menuOpen = Boolean(anchorEl);
  
  // Kullanıcı menüsünü aç
  const handleMenuOpen = (event) => {
    setAnchorEl(event.currentTarget);
  };
  
  // Kullanıcı menüsünü kapat
  const handleMenuClose = () => {
    setAnchorEl(null);
  };
  
  // Drawer'ı aç/kapat
  const toggleDrawer = () => {
    setDrawerOpen(!drawerOpen);
  };
  
  // Menü öğesi tıklama
  const handleNavigation = (path) => {
    navigate(path);
    if (isMobile) {
      setDrawerOpen(false);
    }
  };
  
  // Çıkış yap
  const handleLogout = async () => {
    handleMenuClose();
    await logout();
  };
  
  // Profil sayfasına git
  const handleProfile = () => {
    handleMenuClose();
    navigate('/profile');
  };
  
  // Ayarlar sayfasına git
  const handleSettings = () => {
    handleMenuClose();
    navigate('/settings');
  };
  
  // Menü öğeleri
  const menuItems = [
    { text: 'Dashboard', icon: <DashboardIcon />, path: '/dashboard' },
    { text: 'Araçlar', icon: <DirectionsCarIcon />, path: '/vehicles' },
    { text: 'Cihazlar', icon: <RouterIcon />, path: '/devices' },
    { text: 'Kullanıcılar', icon: <PeopleIcon />, path: '/users' },
    { text: 'Tenantlar', icon: <BusinessIcon />, path: '/tenants' },
    { text: 'Ayarlar', icon: <SettingsIcon />, path: '/settings' },
  ];

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      {/* App Bar */}
      <AppBar
        position="fixed"
        sx={{
          zIndex: (theme) => theme.zIndex.drawer + 1,
          transition: (theme) => theme.transitions.create(['width', 'margin'], {
            easing: theme.transitions.easing.sharp,
            duration: theme.transitions.duration.leavingScreen,
          }),
          ...(drawerOpen && {
            marginLeft: drawerWidth,
            width: `calc(100% - ${drawerWidth}px)`,
            transition: (theme) => theme.transitions.create(['width', 'margin'], {
              easing: theme.transitions.easing.sharp,
              duration: theme.transitions.duration.enteringScreen,
            }),
          }),
        }}
      >
        <Toolbar>
          <IconButton
            edge="start"
            color="inherit"
            aria-label="open drawer"
            onClick={toggleDrawer}
            sx={{ mr: 2 }}
          >
            {drawerOpen ? <ChevronLeftIcon /> : <MenuIcon />}
          </IconButton>
          
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            Araç Takip Sistemi
          </Typography>
          
          {/* Kullanıcı profil menüsü */}
          <Box sx={{ display: 'flex', alignItems: 'center' }}>
            <Typography variant="body2" sx={{ mr: 2, display: { xs: 'none', sm: 'block' } }}>
              {user?.firstName} {user?.lastName}
            </Typography>
            
            <Tooltip title="Hesap ayarları">
              <IconButton
                onClick={handleMenuOpen}
                size="small"
                sx={{ ml: 2 }}
                aria-controls={menuOpen ? 'account-menu' : undefined}
                aria-haspopup="true"
                aria-expanded={menuOpen ? 'true' : undefined}
              >
                {user?.profileImageUrl ? (
                  <Avatar
                    alt={`${user.firstName} ${user.lastName}`}
                    src={user.profileImageUrl}
                    sx={{ width: 32, height: 32 }}
                  />
                ) : (
                  <Avatar sx={{ width: 32, height: 32, bgcolor: 'primary.main' }}>
                    {user?.firstName?.[0]}{user?.lastName?.[0]}
                  </Avatar>
                )}
              </IconButton>
            </Tooltip>
          </Box>
          
          {/* Kullanıcı menüsü */}
          <Menu
            id="account-menu"
            anchorEl={anchorEl}
            open={menuOpen}
            onClose={handleMenuClose}
            onClick={handleMenuClose}
            transformOrigin={{ horizontal: 'right', vertical: 'top' }}
            anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
            PaperProps={{
              elevation: 3,
              sx: {
                overflow: 'visible',
                filter: 'drop-shadow(0px 2px 8px rgba(0,0,0,0.15))',
                mt: 1.5,
                '& .MuiMenuItem-root': {
                  px: 2,
                  py: 1,
                  my: 0.5,
                  borderRadius: 1,
                  mx: 1,
                },
              },
            }}
          >
            <MenuItem onClick={handleProfile}>
              <ListItemIcon>
                <AccountCircleIcon fontSize="small" />
              </ListItemIcon>
              Profilim
            </MenuItem>
            <MenuItem onClick={handleSettings}>
              <ListItemIcon>
                <SettingsIcon fontSize="small" />
              </ListItemIcon>
              Ayarlar
            </MenuItem>
            <Divider />
            <MenuItem onClick={handleLogout}>
              <ListItemIcon>
                <LogoutIcon fontSize="small" />
              </ListItemIcon>
              Çıkış Yap
            </MenuItem>
          </Menu>
        </Toolbar>
      </AppBar>
      
      {/* Drawer */}
      <Drawer
        variant={isMobile ? 'temporary' : 'persistent'}
        open={drawerOpen}
        onClose={toggleDrawer}
        sx={{
          width: drawerWidth,
          flexShrink: 0,
          '& .MuiDrawer-paper': {
            width: drawerWidth,
            boxSizing: 'border-box',
          },
        }}
      >
        <Toolbar />
        <Box sx={{ overflow: 'auto', mt: 2 }}>
          <List>
            {menuItems.map((item) => (
              <ListItem
                button
                key={item.text}
                onClick={() => handleNavigation(item.path)}
                sx={{
                  borderRadius: '0 20px 20px 0',
                  mx: 1,
                  mb: 0.5,
                  '&:hover': {
                    backgroundColor: 'rgba(25, 118, 210, 0.08)',
                  },
                  '&.Mui-selected': {
                    backgroundColor: 'rgba(25, 118, 210, 0.12)',
                    '&:hover': {
                      backgroundColor: 'rgba(25, 118, 210, 0.16)',
                    },
                  },
                }}
              >
                <ListItemIcon>{item.icon}</ListItemIcon>
                <ListItemText primary={item.text} />
              </ListItem>
            ))}
          </List>
          <Divider sx={{ my: 2 }} />
          <List>
            <ListItem
              button
              onClick={handleLogout}
              sx={{
                borderRadius: '0 20px 20px 0',
                mx: 1,
                '&:hover': {
                  backgroundColor: 'rgba(211, 47, 47, 0.08)',
                },
              }}
            >
              <ListItemIcon>
                <LogoutIcon color="error" />
              </ListItemIcon>
              <ListItemText primary="Çıkış Yap" sx={{ color: 'error.main' }} />
            </ListItem>
          </List>
        </Box>
      </Drawer>
      
      {/* Ana içerik */}
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: '100%',
          height: '100%',
          overflow: 'auto',
          backgroundColor: (theme) => theme.palette.background.default,
        }}
      >
        <Toolbar /> {/* App bar için boşluk */}
        <Outlet /> {/* Çocuk route'lar */}
      </Box>
    </Box>
  );
};

export default MainLayout; 