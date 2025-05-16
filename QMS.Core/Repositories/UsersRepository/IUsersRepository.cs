using QMS.Core.DatabaseContext;
using QMS.Core.Models;
namespace QMS.Core.Repositories.UsersRepository;

public interface IUsersRepository
{
    Task<Users> Login(LoginViewModel loginViewModel);
    Task<Users> LoginWithAdId(string AdId);
    Task<List<UserViewModel>> GetUsersListAsync();
    Task<OperationResult> CreateUserAsync(UserViewModel userViewModel, bool returnCreatedRecord = false);
    Task<OperationResult> UpdateUserAsync(UserViewModel userViewModel, bool returnUpdatedRecord = false);
    Task<OperationResult> DeleteUserAsync(int userId);
    Task<UserViewModel?> GetUserByIdAsync(int userId);
}
