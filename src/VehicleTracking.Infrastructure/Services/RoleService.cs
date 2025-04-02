using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Application.Services;
using VehicleTracking.Domain.Entities;
using VehicleTracking.Domain.Repositories;

namespace VehicleTracking.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IPermissionRepository permissionRepository,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> CreateRoleAsync(RoleCreateDto roleCreateDto)
        {
            if (await RoleNameExistsAsync(roleCreateDto.Name))
            {
                throw new InvalidOperationException($"'{roleCreateDto.Name}' adında bir rol zaten mevcut");
            }

            var role = _mapper.Map<Role>(roleCreateDto);
            await _roleRepository.AddAsync(role);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Rol ID boş olamaz", nameof(id));
            }

            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            await _roleRepository.DeleteAsync(id);
            return true;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<List<RoleDto>>(roles);
        }

        public async Task<RoleDto> GetRoleByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Rol ID boş olamaz", nameof(id));
            }

            var role = await _roleRepository.GetByIdAsync(id);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Rol adı boş olamaz", nameof(name));
            }

            var role = await _roleRepository.GetByNameAsync(name);
            return _mapper.Map<RoleDto>(role);
        }

        public async Task<List<PermissionDto>> GetRolePermissionsAsync(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("Rol ID boş olamaz", nameof(roleId));
            }

            var permissions = await _roleRepository.GetRolePermissionIdsAsync(roleId);
            return _mapper.Map<List<PermissionDto>>(permissions);
        }

        public async Task<RoleDto> UpdateRoleAsync(RoleUpdateDto roleUpdateDto)
        {
            if (roleUpdateDto == null)
            {
                throw new ArgumentNullException(nameof(roleUpdateDto), "Rol güncelleme verisi boş olamaz");
            }

            if (string.IsNullOrEmpty(roleUpdateDto.Id))
            {
                throw new ArgumentException("Rol ID boş olamaz", nameof(roleUpdateDto.Id));
            }

            var existingRole = await _roleRepository.GetByIdAsync(roleUpdateDto.Id);
            if (existingRole == null)
            {
                return null;
            }

            // Eğer rol adı değiştiyse ve yeni ad başka bir rolde kullanılıyorsa hata ver
            if (!string.Equals(existingRole.Name, roleUpdateDto.Name, StringComparison.OrdinalIgnoreCase) &&
                await RoleNameExistsAsync(roleUpdateDto.Name))
            {
                throw new InvalidOperationException($"'{roleUpdateDto.Name}' adında bir rol zaten mevcut");
            }

            _mapper.Map(roleUpdateDto, existingRole);
            await _roleRepository.UpdateAsync(existingRole);
            return _mapper.Map<RoleDto>(existingRole);
        }

        public async Task<bool> AssignPermissionToRoleAsync(string roleId, string permissionId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId))
                {
                    throw new ArgumentException("Rol ID boş olamaz", nameof(roleId));
                }

                if (string.IsNullOrEmpty(permissionId))
                {
                    throw new ArgumentException("İzin ID boş olamaz", nameof(permissionId));
                }

                // Rol ve iznin var olup olmadığını kontrol et
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return false;
                }

                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return false;
                }

                // İzin rol ile ilişkilendiriliyor
                return await _roleRepository.AddPermissionToRoleAsync(roleId, permissionId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemovePermissionFromRoleAsync(string roleId, string permissionId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId))
                {
                    throw new ArgumentException("Rol ID boş olamaz", nameof(roleId));
                }

                if (string.IsNullOrEmpty(permissionId))
                {
                    throw new ArgumentException("İzin ID boş olamaz", nameof(permissionId));
                }

                // Rol ve iznin var olup olmadığını kontrol et
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return false;
                }

                var permission = await _permissionRepository.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return false;
                }

                // İzin rolden kaldırılıyor
                return await _roleRepository.RemovePermissionFromRoleAsync(roleId, permissionId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RoleExistsAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Rol ID boş olamaz", nameof(id));
            }

            var role = await _roleRepository.GetByIdAsync(id);
            return role != null;
        }

        public async Task<bool> RoleNameExistsAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Rol adı boş olamaz", nameof(name));
            }

            var role = await _roleRepository.GetByNameAsync(name);
            return role != null;
        }
    }
}