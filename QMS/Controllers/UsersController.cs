using Microsoft.AspNetCore.Mvc;
using QMS.Core.Models;
using QMS.Core.Repositories.UserRolesRepository;
using QMS.Core.Repositories.UsersRepository;

namespace QMS.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUserRolesRepository userRolesRepository,
                               IUsersRepository usersRepository)
        {
            _userRolesRepository = userRolesRepository;
            _usersRepository = usersRepository;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Role = await _userRolesRepository.GetAllAsync();
            var model = await _usersRepository.GetUsersListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var model = await _usersRepository.GetUsersListAsync();
            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var operationResult = await _usersRepository.CreateUserAsync(model);
                return Json(operationResult);
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { Success = false, Errors = errors });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserAsync(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var operationResult = await _usersRepository.UpdateUserAsync(model);
                return Json(operationResult);
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { Success = false, Errors = errors });
        }

        //public async Task<IActionResult> DeleteUserAsync(int id)
        //{
        //    var userExistInCPRLog = await _cprLogRepository.GetCPRLogByUserIdAsync(id);
        //    var userExistInTDS = await _cprLogRepository.GetCPRLogByUserIdAsync(id);
        //    var operationResult = new OperationResult();
        //    if (userExistInCPRLog.Id == 0 && userExistInTDS.Id == 0)
        //    {
        //        operationResult = await _usersRepository.DeleteUserAsync(id);
        //    }
        //    else
        //    {
        //        operationResult = new OperationResult
        //        {
        //            Success = false,
        //            Message = "User is linked with CPR Log or TDS. So, It can't be deleted."
        //        };
        //    }    
        //    return Json(operationResult);
        //}

        public async Task<ActionResult> GetUserByIdAsync(int userId)
        {
            var user = await _usersRepository.GetUserByIdAsync(userId);
            return Json(user);
        }
    }
}
