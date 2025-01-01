using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalMe.Client.Models;
using SignalMe.Data;

namespace SignalMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UserController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        [HttpGet("getAppUserlist")]
        public async Task<ActionResult<List<AppUser>>> GetUserList()
        {
            var users = await _applicationDbContext.Users.ToListAsync();
            var _users = users.Select(user => new AppUser
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LasttName,
                Email = user?.Email
            }).ToList();
            return _users;
        }
    }
}
