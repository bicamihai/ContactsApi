using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ContactsApi.Data;

namespace ContactsApi.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly IApplicationDbContext _applicationDbContext;
        public readonly IContactContext Context;
        protected readonly IMapper Mapper;

        public BaseController(IContactContext contactContext, IApplicationDbContext applicationDbContext, IMapper mapper)
        {
            Context = contactContext;
            _applicationDbContext = applicationDbContext;
            Mapper = mapper;
        }
        private string GetCurrentUser()
        {
            var identity = User.Identity as ClaimsIdentity;
            var identityClaim = identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return  _applicationDbContext.GetUserId(identityClaim?.Value);
        }

        public  virtual string CurrentUserId => GetCurrentUser();
    }
}
