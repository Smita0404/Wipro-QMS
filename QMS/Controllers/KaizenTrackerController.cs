using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.KaizenTrackerRepository;
using QMS.Core.Repositories.VendorRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class KaizenTrackerController : Controller
    {
        private readonly IKaizenTrackerRepository _kaizenTrackerRepository;
        private readonly ISystemLogService _systemLogService;
        private readonly IVendorRepository _vendorRepository;

        public KaizenTrackerController(IKaizenTrackerRepository kaizenTrackerRepository,
                                       ISystemLogService systemLogService,
                                       IVendorRepository vendorRepository)
        {
            _kaizenTrackerRepository = kaizenTrackerRepository;
            _systemLogService = systemLogService;
            _vendorRepository = vendorRepository;
        }

        public IActionResult KaizenTracker()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var list = await _kaizenTrackerRepository.GetListAsync(startDate, endDate);
            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var data = await _kaizenTrackerRepository.GetByIdAsync(id);
            return Json(data);
        }

        [HttpPost]
        [Route("KaizenTracker/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] KaizenTracker model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                // Optional: Add duplication logic
                bool exists = false;

                if (!exists)
                {
                  // model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    var result = await _kaizenTrackerRepository.CreateAsync(model);

                    if (result.Success)
                        return Json(new { success = true, message = "Saved successfully.", id = result.ObjectId });

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
        [Route("KaizenTracker/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] KaizenTracker model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _kaizenTrackerRepository.UpdateAsync(model);

                if (result.Success)
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
        [Route("KaizenTracker/UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file, int kId)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                const long MaxFileSize = 5 * 1024 * 1024;
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                var record = await _kaizenTrackerRepository.GetByIdAsync(kId);
                if (record == null)
                    return NotFound(new { message = "Kaizen record not found." });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "KaizenAttachments");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (file.Length > MaxFileSize)
                    return Json(new { success = false, message = $"File '{file.FileName}' exceeds the 5MB limit." });

                if (!allowedExtensions.Contains(ext))
                    return Json(new { success = false, message = $"File type not allowed: {file.FileName}" });

                var uniqueFileName = $"{record.Vendor}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                string relativePath = Path.Combine("KaizenAttachments", uniqueFileName).Replace("\\", "/");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                record.KaizenFile = relativePath;
                record.Id = kId;

                var updateResult = await _kaizenTrackerRepository.UpdateAsync(record);
                if (!updateResult.Success)
                    return StatusCode(500, new { message = "Failed to update the record with file info." });

                return Json(new { success = true, message = "done" });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, new { message = "File upload or update failed." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            try
            {
                var vendorList = await _kaizenTrackerRepository.GetVendorDropdownAsync();
                return Json(vendorList);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, "Error retrieving vendor dropdown.");
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var result = await _kaizenTrackerRepository.DeleteAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting Kaizen record." });
            }
        }
    }
}
