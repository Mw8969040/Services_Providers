using AutoMapper;
using Smart_Platform.Models;
using Smart_Platform.Services.Interfaces;
using Smart_Platform.UOW;
using Smart_Platform.ViewModel;
using X.PagedList;

namespace Smart_Platform.Services.Implementation
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceRequestService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateAsync(int ServiceId, string CustomerId)
        {
            var service = await _unitOfWork.Repository<Service>().GetByIdAsync(ServiceId);
            if (service == null) throw new Exception("Service Not Found");
            if (service.ProviderId == CustomerId) throw new Exception("Can not request your own service");

            var request = new ServiceRequest()
            {
                CustomerId = CustomerId,
                RequestDate = DateTime.Now,
                requestStatus = RequestStatus.Pending,
                ServiceId = ServiceId,
                TotalPrice = service.BasePrice,
            };

            await _unitOfWork.Repository<ServiceRequest>().AddAsync(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task AcceptAsync(int RequestId, string ProviderId)
        {
            var request = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == RequestId, "Service");
            if (request == null) throw new Exception("Service Request Not Found");
            if (request.Service.ProviderId != ProviderId) throw new Exception("Unauthorized Access To This Request");

            request.requestStatus = RequestStatus.Accepted;
            await _unitOfWork.CompleteAsync();
        }

        public async Task CancelAsync(int RequestId, string CustomerId)
        {
            var request = await _unitOfWork.Repository<ServiceRequest>().GetByIdAsync(RequestId);
            if (request == null) throw new Exception("Service Request Not Found");
            if (request.CustomerId != CustomerId) throw new UnauthorizedAccessException();
            if (request.requestStatus != RequestStatus.Pending) throw new Exception("Only Pending request can be canceled");

            request.requestStatus = RequestStatus.Cancelled;
            _unitOfWork.Repository<ServiceRequest>().Update(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task CompleteAsync(int RequestId, string ProviderId)
        {
            var request = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == RequestId, "Service");
            if (request == null) throw new Exception("Service Request not found");
            if (request.Service.ProviderId != ProviderId || request.requestStatus != RequestStatus.Accepted) throw new UnauthorizedAccessException();

            request.requestStatus = RequestStatus.Completed;
            _unitOfWork.Repository<ServiceRequest>().Update(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task RejectAsync(int RequestId, string ProviderId)
        {
            var request = await _unitOfWork.Repository<ServiceRequest>().GetByIdWithIncludesAsync(r => r.Id == RequestId, "Service");
            if (request == null) throw new Exception("Service Request not found");
            if (request.Service.ProviderId != ProviderId) throw new UnauthorizedAccessException();

            request.requestStatus = RequestStatus.Rejected;
            _unitOfWork.Repository<ServiceRequest>().Update(request);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IPagedList<ServiceRequestVM>> GetRequestsForProviderAsync(string providerId, int pageNumber, int pageSize)
        {
            var requests = await _unitOfWork.Repository<ServiceRequest>().GetPagedWithIncludesAsync(pageNumber, pageSize, X => X.Service.ProviderId == providerId, "Service", "Customer");
            var mappedItems = _mapper.Map<IEnumerable<ServiceRequestVM>>(requests);
            return new StaticPagedList<ServiceRequestVM>(mappedItems, requests);
        }

        public async Task<IPagedList<ServiceRequestVM>> GetRequestsForCustomerAsync(string customerId, int pageNumber, int pageSize)
        {
            var requests = await _unitOfWork.Repository<ServiceRequest>().GetPagedWithIncludesAsync(pageNumber, pageSize, X => X.CustomerId == customerId, "Service", "Customer");
            var mappedItems = _mapper.Map<IEnumerable<ServiceRequestVM>>(requests);
            return new StaticPagedList<ServiceRequestVM>(mappedItems, requests);
        }

        public async Task<bool> HasPendingRequestAsync(int serviceId, string customerId)
        {
            var request = await _unitOfWork.Repository<ServiceRequest>()
                .GetByIdWithIncludesAsync(r => r.ServiceId == serviceId && r.CustomerId == customerId && r.requestStatus == RequestStatus.Pending);
            return request != null;
        }
    }
}
