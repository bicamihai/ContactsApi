using System.Threading.Tasks;

namespace ContactsApi.Data
{
    public interface IApplicationDbContext
    {
        string GetUserId(string id);
    }
}
