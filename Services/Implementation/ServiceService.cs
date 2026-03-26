using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Smart_Platform.Models;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.UOW;
using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Services.Implementation
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment env)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task CreateAsync(ServiceVM serviceVM, string ProviderId)
        {
            var Service = _mapper.Map<Service>(serviceVM);
            Service.ProviderId = ProviderId;
            if (serviceVM.ImageFile != null)
            {
                Service.ImageUrl = await SaveImageAsync(serviceVM.ImageFile);
            }
            await _unitOfWork.Repository<Service>().AddAsync(Service);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int Id, string ProviderId)
        {
            var Service = await _unitOfWork.Repository<Service>().GetByIdAsync(Id);
            if (Service == null) throw new Exception("Service not found");
            if (Service.ProviderId != ProviderId) throw new UnauthorizedAccessException();
            _unitOfWork.Repository<Service>().Delete(Service);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IPagedList<ServiceVM>> GetAllAsync(int pageNumber, int pageSize)
        {
            var Services = await _unitOfWork.Repository<Service>().GetPagedWithIncludesAsync(pageNumber, pageSize, null, "Category", "Provider");
            var mappedItems = _mapper.Map<IEnumerable<ServiceVM>>(Services);
            return new StaticPagedList<ServiceVM>(mappedItems, Services);
        }

        public async Task<IPagedList<ServiceVM>> GetByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            var services = await _unitOfWork.Repository<Service>().GetPagedWithIncludesAsync(pageNumber, pageSize, s => s.CategoryId == categoryId, "Category", "Provider");
            var mappedItems = _mapper.Map<IEnumerable<ServiceVM>>(services);
            return new StaticPagedList<ServiceVM>(mappedItems, services);
        }

        public async Task<ServiceVM?> GetByIdAsync(int Id)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(Id);
            return service == null ? null : _mapper.Map<ServiceVM>(service);
        }

        public async Task<IPagedList<ServiceVM>> GetByProviderAsync(string ProviderId, int pageNumber, int pageSize)
        {
            var Services = await _unitOfWork.Repository<Service>().GetPagedWithIncludesAsync(pageNumber, pageSize, s => s.ProviderId == ProviderId, "Category", "Provider");
            var mappedItems = _mapper.Map<IEnumerable<ServiceVM>>(Services);
            return new StaticPagedList<ServiceVM>(mappedItems, Services);
        }

        public async Task UpdateAsync(int Id, ServiceVM serviceVM, string ProviderId)
        {
            var Service = await _unitOfWork.Repository<Service>().GetByIdAsync(Id);
            if (Service == null) throw new Exception("Service not found");
            if (Service.ProviderId != ProviderId) throw new UnauthorizedAccessException();
            Service.Title = serviceVM.Title;
            Service.Description = serviceVM.Description;
            Service.BasePrice = serviceVM.BasePrice;
            Service.IsAvailable = serviceVM.IsAvailable;
            Service.CategoryId = serviceVM.CategoryId;
            if (serviceVM.ImageFile != null)
            {
                Service.ImageUrl = await SaveImageAsync(serviceVM.ImageFile);
            }
            _unitOfWork.Repository<Service>().Update(Service);
            await _unitOfWork.CompleteAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "services");
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return "/uploads/services/" + uniqueFileName;
        }
    }
}
