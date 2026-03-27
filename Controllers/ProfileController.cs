using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPlatform.Application.DTOs;
using SmartPlatform.Application.Features.Profiles.Commands;
using SmartPlatform.Application.Features.Profiles.Queries;
using System.Security.Claims;

namespace SmartPlatform.Web.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            if (User.IsInRole("Admin"))
            {
                var customerProfile = await _mediator.Send(new GetCustomerProfileQuery(userId));
                if (customerProfile != null) return View("CustomerIndex", customerProfile);
                
                // Admin has no customer profile — show a simple fallback
                return View("CustomerIndex", new CustomerProfileDto { UserId = userId, FullName = "Admin" });
            }

            if (User.IsInRole("Provider"))
            {
                var profile = await _mediator.Send(new GetProviderProfileQuery(userId));
                if (profile == null)
                {
                    // Auto-create missing ProviderProfile for existing providers
                    await _mediator.Send(new CreateProviderProfileCommand
                    {
                        UserId = userId,
                        BusinessName = "General Service",
                        Description = "Professional provider on Smart Platform"
                    });
                    profile = await _mediator.Send(new GetProviderProfileQuery(userId));
                }
                return View("ProviderIndex", profile);
            }

            if (User.IsInRole("Customer"))
            {
                var profile = await _mediator.Send(new GetCustomerProfileQuery(userId));
                if (profile == null)
                {
                    // Auto-create missing CustomerProfile
                    await _mediator.Send(new CreateCustomerProfileCommand { UserId = userId });
                    profile = await _mediator.Send(new GetCustomerProfileQuery(userId));
                }
                return View("CustomerIndex", profile);
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer(CustomerProfileDto profileDto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View("CustomerIndex", profileDto);

            if (imageFile != null && imageFile.Length > 0)
            {
                profileDto.ProfilePictureUrl = await SaveImage(imageFile);
            }
            
            var result = await _mediator.Send(new UpdateCustomerProfileCommand(profileDto));
            if (result)
            {
                TempData["Success"] = "Customer profile updated!";
            }
            else
            {
                TempData["Error"] = "Failed to update profile.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Provider")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProvider(ProviderProfileDto profileDto, IFormFile? imageFile)
        {
            if (!ModelState.IsValid) return View("ProviderIndex", profileDto);

            if (imageFile != null && imageFile.Length > 0)
            {
                profileDto.ProfilePictureUrl = await SaveImage(imageFile);
            }
            
            var result = await _mediator.Send(new UpdateProviderProfileCommand(profileDto));
            if (result)
            {
                TempData["Success"] = "Provider profile updated!";
            }
            else
            {
                TempData["Error"] = "Failed to update profile.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return "/uploads/profiles/" + fileName;
        }

        [AllowAnonymous]
        public async Task<IActionResult> PublicProfile(string providerId)
        {
            if (string.IsNullOrEmpty(providerId)) return NotFound();

            var profile = await _mediator.Send(new GetPublicProfileQuery(providerId));
            if (profile == null) return NotFound();

            return View(profile);
        }
    }
}
