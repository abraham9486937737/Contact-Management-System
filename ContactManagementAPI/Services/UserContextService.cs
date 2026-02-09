using ContactManagementAPI.Data;
using ContactManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContactManagementAPI.Services
{
    public class UserContextService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthorizationService _authorizationService;

        public UserContextService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, AuthorizationService authorizationService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        public int? UserId => _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKeys.UserId);

        public bool IsAuthenticated => UserId.HasValue;

        public AppUser? CurrentUser
        {
            get
            {
                if (!UserId.HasValue)
                    return null;

                return _context.AppUsers
                    .AsNoTracking()
                    .Include(u => u.Group)
                    .FirstOrDefault(u => u.Id == UserId.Value);
            }
        }

        public bool HasRight(string rightKey)
        {
            if (!UserId.HasValue)
                return false;

            return _authorizationService.HasRight(UserId.Value, rightKey);
        }

        public bool IsAdmin => UserId.HasValue && _authorizationService.IsAdmin(UserId.Value);
    }
}
