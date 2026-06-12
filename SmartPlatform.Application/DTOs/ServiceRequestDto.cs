using System;
using SmartPlatform.Domain.Entities;
using SmartPlatform.Application.DTOs.Enums;
namespace SmartPlatform.Application.DTOs
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceTitle { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhoneNumber { get; set; }
        public string? CustomerAddress { get; set; }
        public DateTime RequestDate { get; set; }
        public RequestStatusDto requestStatus { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public bool IsReviewed { get; set; }
        
        public DateTime CreatedDate => RequestDate;
    }
}
