using QMS.Core.DatabaseContext;

namespace QMS.Core.Repositories.UserRolesRepository;

public interface IUserRolesRepository
{
    Task<List<UserRoles>> GetAllAsync();
}
