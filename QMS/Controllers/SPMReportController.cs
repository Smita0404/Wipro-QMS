using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.SPMReportRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class SPMReportController : Controller
    {
        private readonly ISPMReportRepository _spmReportRepository;
        private readonly ISystemLogService _systemLogService;
        private readonly IVendorRepository _vendorRepository;

        public SPMReportController(ISPMReportRepository spmReportRepository, ISystemLogService systemLogService, IVendorRepository vendorRepository)
        {
            _spmReportRepository = spmReportRepository;
            _systemLogService = systemLogService;
            _vendorRepository = vendorRepository;
        }

        public IActionResult SPMReport()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var list = await _spmReportRepository.GetListAsync(startDate, endDate);
            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var item = await _spmReportRepository.GetByIdAsync(id);
            return Json(item);
        }

        [HttpPost]
        [Route("SPMReport/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] SPMReport model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                var operationResult = new OperationResult();
                bool exists = false; // Optional: Add duplicate check logic

                if (!exists)
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    operationResult = await _spmReportRepository.CreateAsync(model);

                    if (operationResult != null && operationResult.Success)
                        return Json(new { success = true, message = "Saved successfully.", id = operationResult.ObjectId });

                    return Json(new { success = false, message = "Failed to save.", id = 0 });
                }
                else
                {
                    return Json(new { success = false, message = "Duplicate entry." });
                }
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while saving." });
            }
        }

        [HttpPost]
        [Route("SPMReport/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] SPMReport model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _spmReportRepository.UpdateAsync(model);

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

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var operationResult = await _spmReportRepository.DeleteAsync(id);
                return Json(operationResult);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting SPM Report." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            try
            {
                var vendorList = await _spmReportRepository.GetVendorDropdownAsync();
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
