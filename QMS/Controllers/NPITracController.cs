using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.NPITrackerRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class NPITracController : Controller
    {
        private readonly INPITrackerRepository _nPITarcRepository;
        private readonly ISystemLogService _systemLogService;
        private readonly IVendorRepository _vendorRepository;

        public NPITracController(INPITrackerRepository nPITarcRepository, ISystemLogService systemLogService,IVendorRepository vendorRepository)
        {
            _nPITarcRepository = nPITarcRepository;
            _systemLogService = systemLogService;
            _vendorRepository = vendorRepository;
        }
        public IActionResult NPITracker()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var bisProjectList = await _nPITarcRepository.GetListAsync();
            return Json(bisProjectList);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int Id)
        {
            var bisProject = await _nPITarcRepository.GetByIdAsync(Id);
            return Json(bisProject);
        }

        [HttpPost]
        public async Task<JsonResult> CreateAsync(NPITracker model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var operationResult = new OperationResult();
                    //bool existingResult = await _bisProjectRepository.CheckDuplicate(model.Name.Trim(), 0);
                    bool existingResult = true;
                    if (!existingResult)
                    {
                        model.CreatedDate = DateTime.Now;
                        model.CreatedBy = HttpContext.Session.GetString("FullName");
                        operationResult = await _nPITarcRepository.CreateAsync(model);

                        if (operationResult != null)
                        {
                            return Json(new { success = true, message = "NPI Tracker Detail saved successfully.", id = operationResult.ObjectId });
                        }

                        return Json(new { success = false, message = "Failed to save npi tracker detail.", id = 0 });
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
        public async Task<JsonResult> UpdateAsync(NPITracker model)
        {
            if (ModelState.IsValid)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var operationResult = await _nPITarcRepository.UpdateAsync(model);

                return Json(operationResult);
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return Json(new { Success = false, Errors = errors });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            var operationResult = await _nPITarcRepository.DeleteAsync(id);
            return Json(operationResult);
        }

        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            try
            {
                var vendorList = await _vendorRepository.GetVendorDropdownAsync();
                return Json(vendorList); 
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, "Error retrieving vendor dropdown.");
            }
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

    }
}
