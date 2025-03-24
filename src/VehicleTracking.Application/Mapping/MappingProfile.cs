using AutoMapper;
using MongoDB.Driver.GeoJsonObjectModel;
using VehicleTracking.Application.DTOs;
using VehicleTracking.Domain.Entities;

namespace VehicleTracking.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Vehicle, VehicleDto>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => 
                    src.Location != null ? src.Location.Coordinates.Latitude : 0))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => 
                    src.Location != null ? src.Location.Coordinates.Longitude : 0));

            CreateMap<VehicleDto, Vehicle>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => 
                    new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                        new GeoJson2DGeographicCoordinates(src.Longitude, src.Latitude))));
        }
    }
} 