using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.DNTrackerRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class DNTrackerController: Controller
    {
        private readonly IDNTrackerRepository _deviationNoteRepository;
        private readonly ISystemLogService _systemLogService;

        public DNTrackerController(IDNTrackerRepository deviationNoteRepository, ISystemLogService systemLogService)
        {
            _deviationNoteRepository = deviationNoteRepository;
            _systemLogService = systemLogService;
        }

        public IActionResult DNTracker()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var list = await _deviationNoteRepository.GetListAsync();
            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var item = await _deviationNoteRepository.GetByIdAsync(id);
            return Json(item);
        }

        [HttpPost]
        [Route("DNTracker/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] DNTracker model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                var operationResult = new OperationResult();
                bool exists = false; // Optional: Add duplicate check here

                if (!exists)
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    operationResult = await _deviationNoteRepository.CreateAsync(model);

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
        [Route("DNTracker/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] DNTracker model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _deviationNoteRepository.UpdateAsync(model);

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
                var operationResult = await _deviationNoteRepository.DeleteAsync(id);
                return Json(operationResult);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting Deviation Note." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetVendors()
        {
            try
            {
                var vendorList = await _deviationNoteRepository.GetVendorDropdownAsync();
                return Json(vendorList);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, "Error retrieving vendor dropdown.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductCodes()
        {
            var productCodes = await _deviationNoteRepository.GetProductCodeAsync();
            return Json(productCodes);
        }

    }
}
