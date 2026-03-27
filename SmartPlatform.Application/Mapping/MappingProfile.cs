using AutoMapper;
using SmartPlatform.Domain.Entities;
using SmartPlatform.Application.DTOs;

namespace SmartPlatform.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider.FullName));

            CreateMap<ServiceDto, Service>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceRequests, opt => opt.Ignore());

            CreateMap<ServiceCategory, CategoryDto>()
                .ForMember(dest => dest.ServicesCount, opt => opt.MapFrom(src => src.Services.Count));

            CreateMap<CategoryDto, ServiceCategory>()
                .ForMember(dest => dest.Services, opt => opt.Ignore());

            CreateMap<ServiceRequest, ServiceRequestDto>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName));

            CreateMap<ServiceRequestDto, ServiceRequest>()
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore());

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.ServiceRequest.Service.Title));

            CreateMap<ReviewDto, Review>()
                .ForMember(dest => dest.ServiceRequest, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserDto>();
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.Services, opt => opt.Ignore())
                .ForMember(dest => dest.Requests, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerProfile, opt => opt.Ignore())
                .ForMember(dest => dest.ProviderProfile, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // Usually don't map Id back

            CreateMap<CustomerProfile, CustomerProfileDto>();
            CreateMap<CustomerProfileDto, CustomerProfile>()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<ProviderProfile, ProviderProfileDto>();
            CreateMap<ProviderProfileDto, ProviderProfile>()
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }
    }
}
