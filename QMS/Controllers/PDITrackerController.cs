using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.PDITrackerRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class PDITrackerController : Controller
    {
        private readonly IPDITrackerRepository _pdiTrackerRepository;
        private readonly ISystemLogService _systemLogService;
        private readonly IVendorRepository _vendorRepository;
        public PDITrackerController(IPDITrackerRepository pdiTrackerRepository, ISystemLogService systemLogService, IVendorRepository vendorRepository)
        {
            _pdiTrackerRepository = pdiTrackerRepository;
            _systemLogService = systemLogService;
            _vendorRepository = vendorRepository;
        }

        public IActionResult PDITracker()
        {
            return View();
        }
        [HttpGet]
        public async Task<JsonResult> GetCodeSearchAsync(string search = "")
        {
            try
            {
                // Initialize processed search terms
                string processedSearch = string.Empty;

                if (!string.IsNullOrEmpty(search))
                {
                    if (search.Length >= 4)
                        processedSearch = search.Substring(0, 4); // First 4 characters
                }

                var productCodeDetailsList = await _vendorRepository.GetCodeSearchAsync(processedSearch);

                return Json(productCodeDetailsList);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var list = await _pdiTrackerRepository.GetListAsync(startDate, endDate);
            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var item = await _pdiTrackerRepository.GetByIdAsync(id);
            return Json(item);
        }

        [HttpPost]
        [Route("PDITracker/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] PDITracker model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                var operationResult = new OperationResult();
                bool exists = false; // Add duplicate check if needed

                if (!exists)
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    operationResult = await _pdiTrackerRepository.CreateAsync(model);

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
        [Route("PDITracker/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] PDITracker model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _pdiTrackerRepository.UpdateAsync(model);

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
       
        [HttpGet]
        public async Task<IActionResult> GetProductCodes()
        {
            var productCodes = await _pdiTrackerRepository.GetCodeSelect2OptionsAsync();
               return Json(productCodes);
        }



        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var operationResult = await _pdiTrackerRepository.DeleteAsync(id);
                return Json(operationResult);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting PDI tracker." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            try
            {
                var vendorList = await _pdiTrackerRepository.GetVendorDropdownAsync();
                return Json(vendorList);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, "Error retrieving vendor dropdown.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductCodeSearch(string search)
        {
            var data = await _pdiTrackerRepository.GetCodeSearchAsync(search);
            return Json(data);
        }

    }
}
