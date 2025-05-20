using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.CSOTrackerRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class CSOTrackerController : Controller
    {
        private readonly ICSOTrackerRepository _csoTrackerRepository;
        private readonly ISystemLogService _systemLogService;
        private readonly IVendorRepository _vendorRepository;

        public CSOTrackerController(ICSOTrackerRepository csoTrackerRepository, ISystemLogService systemLogService, IVendorRepository vendorRepository)
        {
            _csoTrackerRepository = csoTrackerRepository;
            _systemLogService = systemLogService;
            _vendorRepository = vendorRepository;
        }

        public IActionResult CSOTracker()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            var csoList = await _csoTrackerRepository.GetListAsync();
            return Json(csoList);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var cso = await _csoTrackerRepository.GetByIdAsync(id);
            return Json(cso);
        }
        [HttpPost]
        [Route("CSOTracker/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] CSOTracker model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                var operationResult = new OperationResult();
                bool exists = false; // Your duplicate check here

                if (!exists)
                {
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    operationResult = await _csoTrackerRepository.CreateAsync(model);

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
        [Route("CSOTracker/UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file, int csoId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                const long MaxFileSize = 5 * 1024 * 1024;
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var record = await _csoTrackerRepository.GetByIdAsync(csoId);
                if (record == null)
                    return NotFound(new { message = "CSO record not found." });
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CSOTrackerAttachments");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (file.Length > MaxFileSize)
                    return Json(new { success = false, message = $"File '{file.FileName}' exceeds the 5MB limit." });

                if (!allowedExtensions.Contains(ext))
                    return Json(new { success = false, message = $"File type not allowed: {file.FileName}" });
                var uniqueFileName = $"{record.CSONo}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                string relativePath = Path.Combine("CSOTrackerAttachments", uniqueFileName).Replace("\\", "/");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Update the record in your DB (example: using your repository)
               

                record.AttachmentCAPAReport = relativePath;
                record.Id = csoId;
                var updateResult = await _csoTrackerRepository.UpdateAsync(record);

                if (!updateResult.Success)
                {
                    return StatusCode(500, new { message = "Failed to update the record with file info." });
                }
                else
                {
                    return Json(new { success = true, message = "done" });
                }

               // return Ok(new { filename = uniqueFileName });
            }
            catch (Exception ex)
            {
                // Log error as needed
                return StatusCode(500, new { message = "File upload or update failed." });
            }
        }

        [HttpPost]
        [Route("CSOTracker/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] CSOTracker model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _csoTrackerRepository.UpdateAsync(model);

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
        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var operationResult = await _csoTrackerRepository.DeleteAsync(id);
                return Json(operationResult);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting CSO tracker." });
            }
        }

       
    }
}
