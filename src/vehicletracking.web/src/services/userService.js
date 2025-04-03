import api from './api';

const userService = {
  // Tüm kullanıcıları getir
  getAllUsers: async (page = 1, pageSize = 10, search = '') => {
    const response = await api.get('/users', {
      params: { page, pageSize, search }
    });
    return response.data;
  },
  
  // Kullanıcı detaylarını getir
  getUserById: async (id) => {
    const response = await api.get(`/users/${id}`);
    return response.data;
  },
  
  // Kullanıcı oluştur
  createUser: async (userData) => {
    const response = await api.post('/users', userData);
    return response.data;
  },
  
  // Kullanıcı güncelle
  updateUser: async (id, userData) => {
    const response = await api.put(`/users/${id}`, userData);
    return response.data;
  },
  
  // Kullanıcı sil
  deleteUser: async (id) => {
    const response = await api.delete(`/users/${id}`);
    return response.data;
  },
  
  // Kullanıcıyı aktifleştir
  activateUser: async (id) => {
    const response = await api.patch(`/users/${id}/activate`);
    return response.data;
  },
  
  // Kullanıcıyı deaktif et
  deactivateUser: async (id) => {
    const response = await api.patch(`/users/${id}/deactivate`);
    return response.data;
  },
  
  // Kullanıcı rollerini güncelle
  updateUserRoles: async (id, roles) => {
    const response = await api.put(`/users/${id}/roles`, { roles });
    return response.data;
  },
  
  // Mevcut kullanıcının profil bilgilerini getir
  getCurrentUserProfile: async () => {
    const response = await api.get('/users/profile');
    return response.data;
  },
  
  // Mevcut kullanıcının profil bilgilerini güncelle
  updateCurrentUserProfile: async (profileData) => {
    const response = await api.put('/users/profile', profileData);
    return response.data;
  },
  
  // Kullanıcı profilini resmi yükle
  uploadProfileImage: async (imageFile) => {
    const formData = new FormData();
    formData.append('image', imageFile);
    
    const response = await api.post('/users/profile/image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },
  
  // Kullanıcıya atanmış tüm izinleri getir
  getUserPermissions: async (id) => {
    const response = await api.get(`/users/${id}/permissions`);
    return response.data;
  },
  
  // Kullanıcıyı belirli bir role ata
  assignRoleToUser: async (userId, roleId) => {
    const response = await api.post(`/users/${userId}/roles/${roleId}`);
    return response.data;
  },
  
  // Kullanıcıdan belirli bir rolü kaldır
  removeRoleFromUser: async (userId, roleId) => {
    const response = await api.delete(`/users/${userId}/roles/${roleId}`);
    return response.data;
  },
  
  // Tüm rolleri getir
  getAllRoles: async () => {
    const response = await api.get('/roles');
    return response.data;
  }
};

export default userService; 