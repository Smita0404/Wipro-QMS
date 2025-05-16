using Microsoft.AspNetCore.Mvc;
using QMS.Core.Repositories.DashBordRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISystemLogService _systemLogService;
        private readonly IDashBoardRepository _dashBoardRepository;

        public DashboardController(ISystemLogService systemLogService, IDashBoardRepository dashBoardRepository)
        {
            _systemLogService = systemLogService;
            _dashBoardRepository = dashBoardRepository;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}
