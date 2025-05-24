using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.COPQComplaintDumpRepository;
using QMS.Core.Services.SystemLogs;
using System;
using System.Threading.Tasks;

namespace QMS.Controllers
{
    public class COPQController : Controller
    {
        private readonly ICOPQComplaintDumpRepository _copqRepository;
        private readonly ISystemLogService _systemLogService;

        public COPQController(ICOPQComplaintDumpRepository copqRepository, ISystemLogService systemLogService)
        {
            _copqRepository = copqRepository;
            _systemLogService = systemLogService;
        }

        // Main page/view
        public IActionResult COPQ()
        {
            return View();
        }

        // Get all records with optional filtering by date range
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var list = await _copqRepository.GetListAsync();
            return Json(list);
        }

        // Get a record by id
        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var item = await _copqRepository.GetByIdAsync(id);
            return Json(item);
        }

        // Create a new record
        [HttpPost]
        [Route("COPQ/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] COPQComplaintDump model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = HttpContext.Session.GetString("FullName");
                model.Deleted = false;

                var operationResult = await _copqRepository.CreateAsync(model);

                if (operationResult != null && operationResult.Success)
                    return Json(new { success = true, message = "Saved successfully." });

                return Json(new { success = false, message = "Failed to save." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while saving." });
            }
        }

        // Update existing record
        [HttpPost]
        [Route("COPQ/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] COPQComplaintDump model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _copqRepository.UpdateAsync(model);

                if (result != null && result.Success)
                    return Json(new { success = true, message = "Updated successfully." });

                return Json(new { success = false, message = "Update failed." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error during update." });
            }
        }

        // Delete a record by id
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var operationResult = await _copqRepository.DeleteAsync(id);
                return Json(operationResult);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting record." });
            }
        }
    }
}
