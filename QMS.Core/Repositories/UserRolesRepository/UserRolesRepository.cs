using QMS.Core.DatabaseContext;
using QMS.Core.Services.SystemLogs;
using Microsoft.EntityFrameworkCore;
namespace QMS.Core.Repositories.UserRolesRepository;

public class UserRolesRepository : IUserRolesRepository
{
    private readonly QMSDbContext _dbContext;
    private readonly ISystemLogService _systemLogService;

    public UserRolesRepository(QMSDbContext dbContext,
                               ISystemLogService systemLogService)
    {
        _dbContext = dbContext;
        _systemLogService = systemLogService;
    }

    public async Task<List<UserRoles>> GetAllAsync()
    {
        try
        {
            var list = await _dbContext.UserRole.Where(i => i.Deleted == false).ToListAsync();
            return list;
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }    
    }
}
