using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.BisProjectTracRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class BisProjectTracController : Controller
    {
        private readonly IBisProjectTracRepository _bisProjectRepository;
        private readonly ISystemLogService _systemLogService;

        public BisProjectTracController(IBisProjectTracRepository bisProjectRepository, ISystemLogService systemLogService)
        {
            _bisProjectRepository = bisProjectRepository;
            _systemLogService = systemLogService;
        }

        public IActionResult BisProjectTracker()
        {
            return View();
        }

        public async Task<IActionResult> BisProjectTrackerDetail(int id)
        {
            var model = new BisProjectTracViewModel();

            if (id > 0)
            {
                model = await _bisProjectRepository.GetByIdAsync(id);
                if (model == null)
                {
                    model = new BisProjectTracViewModel();
                }
            }
            else
            {
                model = new BisProjectTracViewModel();
            }

            return View(model); // Always return the model, whether new or fetched
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var bisProjectList = await _bisProjectRepository.GetListAsync();
            return Json(bisProjectList);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int Id)
        {
            var bisProject = await _bisProjectRepository.GetByIdAsync(Id);
            return Json(bisProject);
        }

        [HttpPost]
        public async Task<JsonResult> CreateAsync(BisProject_Tracker model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var operationResult = new OperationResult();
                    //bool existingResult = await _bisProjectRepository.CheckDuplicate(model.Name.Trim(), 0);
                    bool existingResult = false;
                    if (!existingResult)
                    {
                        model.CreatedDate = DateTime.Now;
                        model.CreatedBy = HttpContext.Session.GetString("FullName");
                        operationResult = await _bisProjectRepository.CreateAsync(model);

                        if (operationResult != null)
                        {
                            return Json(new { success = true, message = "Bis Project Detail saved successfully.", id = operationResult.ObjectId });
                        }

                        return Json(new { success = false, message = "Failed to save bis project detail.", id = 0 });
                    }
                    else
                    {
                        operationResult.Success = false;
                        operationResult.Message = "Exist";
                        operationResult.Payload = existingResult;
                        return Json(operationResult);
                    }
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { Success = false, Errors = errors });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<JsonResult> UpdateAsync(BisProject_Tracker model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var operationResult = await _bisProjectRepository.UpdateAsync(model);

                return Json(operationResult);
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { Success = false, Errors = errors });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var operationResult = await _bisProjectRepository.DeleteAsync(id);
            return Json(operationResult);
        }
    }
}
