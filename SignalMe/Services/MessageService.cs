using SignalMe.Data;
using SignalMe.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SignalMe.Services
{
    public class MessageService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext)
        {
            _db = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

       
        public async Task<string> GetLoggedinUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId ?? string.Empty;
        }
    }
}
