using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using ContactsApi.Data;

namespace ContactsApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public BaseController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        internal string GetCurrentUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var identityClaim = identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return _applicationDbContext.Users.FirstOrDefault(u => u.Id == identityClaim.Value)?.Id;
        }
    }
}
