using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using ContactsApi.Data;

namespace ContactsApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        protected readonly IMapper Mapper;

        public BaseController(ApplicationDbContext applicationDbContext, IMapper mapper)
        {
            _applicationDbContext = applicationDbContext;
            Mapper = mapper;
        }
        private string GetCurrentUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var identityClaim = identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return _applicationDbContext.Users.FirstOrDefault(u => u.Id == identityClaim.Value)?.Id;
        }

        public virtual string CurrentUserId
        {
            get
            {
                return GetCurrentUser();
            }
        }
    }
}
