using AutoMapper;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>().ReverseMap();
        }
    }
} 