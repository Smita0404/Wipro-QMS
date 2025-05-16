using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using Microsoft.EntityFrameworkCore;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;

namespace QMS.Core.Repositories.UsersRepository;

public class UsersRepository : SqlTableRepository, IUsersRepository
{
    private new readonly QMSDbContext _dbContext;
    private readonly ISystemLogService _systemLogService;

    public UsersRepository(QMSDbContext dbContext,
                           ISystemLogService systemLogService) : base(dbContext)
    {
        _dbContext = dbContext;
        _systemLogService = systemLogService;
    }

    public async Task<Users> Login(LoginViewModel loginViewModel)
    {
        try
        {
            var result = await _dbContext.User.FirstOrDefaultAsync(x => x.Username == loginViewModel.Username && x.Password == loginViewModel.Password);
            return result;
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<Users> LoginWithAdId(string AdId)
    {
        try
        {
            var result = await _dbContext.User.FirstOrDefaultAsync(x => x.AdId == AdId);
            return result;
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<List<UserViewModel>> GetUsersListAsync()
    {
        try
        {
            var result = await _dbContext.User.Include(i => i.UserRoles).Where(x => x.Deleted == false).ToListAsync();

            var list = result.Select(x => new UserViewModel { 
                UserId = x.Id,
                Name = x.Name,
                Email = x.Email,
                RoleId = x.RoleId,
                Role = x.UserRoles?.RoleName,
                AdId = x.AdId,
                Designation = x.Designation,
                DivisionId = x.DivisionId,
                MobileNo = x.MobileNo,
                User_Type = x.User_Type
            }).ToList();

            return list;
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<OperationResult> CreateUserAsync(UserViewModel userViewModel, bool returnCreatedRecord = false)
    {
        try
        {
            var userRecordToCreate = new Users
            {
                Name = userViewModel.Name,
                Email = userViewModel.Email,
                RoleId = userViewModel.RoleId,
                AdId = userViewModel.AdId,
                Username = userViewModel.AdId,
                Password = userViewModel.AdId,
                Designation = userViewModel.Designation,
                DivisionId = userViewModel.DivisionId,
                MobileNo = userViewModel.MobileNo,
                User_Type = userViewModel.User_Type
            };
            return await base.CreateAsync<Users>(userRecordToCreate, returnCreatedRecord);
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<OperationResult> UpdateUserAsync(UserViewModel userViewModel, bool returnUpdatedRecord = false)
    {
        try
        {
            var userRecordToUpdate = await base.GetByIdAsync<Users>(userViewModel.UserId);
            userRecordToUpdate.Name = userViewModel.Name;
            userRecordToUpdate.Email = userViewModel.Email;
            userRecordToUpdate.RoleId = userViewModel.RoleId;
            userRecordToUpdate.AdId = userViewModel.AdId;
            userRecordToUpdate.Username = userViewModel.AdId;
            userRecordToUpdate.Password = userViewModel.AdId;
            userRecordToUpdate.Designation = userViewModel.Designation;
            userRecordToUpdate.DivisionId = userViewModel.DivisionId;
            userRecordToUpdate.MobileNo = userViewModel.MobileNo;
            userRecordToUpdate.User_Type = userViewModel.User_Type;
            return await base.UpdateAsync<Users>(userRecordToUpdate, returnUpdatedRecord);
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<OperationResult> DeleteUserAsync(int userId)
    {
        try
        {
            return await base.DeleteAsync<Users>(userId);
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }

    public async Task<UserViewModel?> GetUserByIdAsync(int userId)
    {
        try
        {
            var userDetails = await base.GetByIdAsync<Users>(userId);
            var user = new UserViewModel();
            if (userDetails != null) 
            {
                user.UserId = userDetails.Id;
                user.Name = userDetails.Name;
                user.Email = userDetails.Email;
                user.RoleId = userDetails.RoleId;
                user.AdId = userDetails.AdId;
                user.Designation = userDetails.Designation;
                user.DivisionId = userDetails.DivisionId;
                user.MobileNo = userDetails.MobileNo;
                user.User_Type = userDetails.User_Type;
            }
            return user;
        }
        catch (Exception ex)
        {
            _systemLogService.WriteLog(ex.Message);
            throw;
        }
    }
}
