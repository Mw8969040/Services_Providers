using AutoMapper;
using Smart_Platform.Models;
using Smart_Platform.ViewModel;

namespace Smart_Platform.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Service, ServiceVM>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider.FullName));

            CreateMap<ServiceVM, Service>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceRequests, opt => opt.Ignore());

            CreateMap<ServiceCategory, CategoryVM>()
                .ForMember(dest => dest.ServicesCount, opt => opt.MapFrom(src => src.Services.Count));

            CreateMap<CategoryVM, ServiceCategory>()
                .ForMember(dest => dest.Services, opt => opt.Ignore());

            CreateMap<ServiceRequest, ServiceRequestVM>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.Service.Title))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName));

            CreateMap<ServiceRequestVM, ServiceRequest>()
                .ForMember(dest => dest.Service, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore());

            CreateMap<Review, ReviewVM>()
                .ForMember(dest => dest.ServiceTitle, opt => opt.MapFrom(src => src.ServiceRequest.Service.Title));

            CreateMap<ReviewVM, Review>()
                .ForMember(dest => dest.ServiceRequest, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserVM>().ReverseMap();
        }
    }
}
