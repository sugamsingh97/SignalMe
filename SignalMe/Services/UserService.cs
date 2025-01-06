using SignalMe.Data;
using SignalMe.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SignalMe.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _db = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ActionResult<List<AppUser>>> GetUserList()
        {
            var userId = await GetLoggedinUserId();
            var users = await _db.Users.ToListAsync();
            var _users = users.Select(user => new AppUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LasttName,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber,
            }).ToList();
            return _users;
        }

        public async Task<string> GetLoggedinUserId()
        {
            var userId =  _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId ?? string.Empty;
        }

        public async Task<AppUser?> SearchUserByEmail(string email)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            if (user == null) return null;

            return new AppUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LasttName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> UserExists(string userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return user.Id != "" ? true : false;
        }
    }
}
