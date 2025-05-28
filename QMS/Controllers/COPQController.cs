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
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var list = await _copqRepository.GetListAsync(startDate, endDate);
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


        // Get PO list with optional date range
        [HttpGet]
        public async Task<JsonResult> GetAllPO(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var list = await _copqRepository.GetPOListAsync(startDate, endDate);
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetAll PO Error: {ex.Message}");
                return Json(new { success = false, message = "Failed to load PO list." });
            }
        }

        // GET: /PO/GetById/5
        [HttpGet]
        public async Task<JsonResult> GetByIdPO(int id)
        {
            try
            {
                var po = await _copqRepository.GetPOByIdAsync(id);
                if (po == null)
                    return Json(new { success = false, message = "PO not found." });

                return Json(new { success = true, data = po });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetById PO Error: {ex.Message}");
                return Json(new { success = false, message = "Failed to get PO details." });
            }
        }
        [HttpPost]
        public async Task<JsonResult> CreatePO([FromBody] PODetail model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid PO data." });

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = HttpContext.Session.GetString("FullName") ?? "System";
                model.Deleted = false;

                var result = await _copqRepository.CreatePOAsync(model);
                if (result.Success)
                    return Json(new { success = true, message = "PO created successfully." });

                return Json(new { success = false, message = result.Message ?? "Failed to create PO." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Create PO Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while creating PO." });
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdatePO([FromBody] PODetail model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid PO update data." });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName") ?? "System";

                var result = await _copqRepository.UpdatePOAsync(model);
                if (result.Success)
                    return Json(new { success = true, message = "PO updated successfully." });

                return Json(new { success = false, message = result.Message ?? "Failed to update PO." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Update PO Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while updating PO." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> PODelete(int id)
        {
            try
            {
                var result = await _copqRepository.DeletePOAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Delete PO Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while deleting PO." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            try
            {
                var vendorList = await _copqRepository.GetVendorDropdownAsync();
                return Json(vendorList);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, "Error retrieving vendor dropdown.");
            }
        }
    }
}
