using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.RMTCDetailsRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class RMTCController : Controller
    {
        private readonly IRMTCDetailsRepository _rmtcRepository;
        private readonly ISystemLogService _systemLogService;

        public RMTCController(IRMTCDetailsRepository rmtcRepository, ISystemLogService systemLogService)
        {
            _rmtcRepository = rmtcRepository;
            _systemLogService = systemLogService;
        }

        public IActionResult RMTC()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var list = await _rmtcRepository.GetListAsync(startDate, endDate);
                return Json(list);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error fetching RMTC details." });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var item = await _rmtcRepository.GetByIdAsync(id);
                return Json(item);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error fetching record." });
            }
        }

        [HttpPost]
        [Route("RMTC/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] RMTCDetails model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid data" });

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = HttpContext.Session.GetString("FullName");

                var result = await _rmtcRepository.CreateAsync(model);
                if (result.Success)
                    return Json(new { success = true, message = "Saved successfully.", id = result.ObjectId });

                return Json(new { success = false, message = "Failed to save." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("RMTC/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] RMTCDetails model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid update data" });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName");

                var result = await _rmtcRepository.UpdateAsync(model);
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
        [Route("RMTC/UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file, int Id)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                const long MaxFileSize = 5 * 1024 * 1024;
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                var record = await _rmtcRepository.GetByIdAsync(Id);
                if (record == null)
                    return NotFound(new { message = "record not found." });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "RMTCAttachments");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var ext = Path.GetExtension(file.FileName).ToLower();
                if (file.Length > MaxFileSize)
                    return Json(new { success = false, message = $"File '{file.FileName}' exceeds the 5MB limit." });

                if (!allowedExtensions.Contains(ext))
                    return Json(new { success = false, message = $"File type not allowed: {file.FileName}" });

                var uniqueFileName = $"{record.Vendor}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                string relativePath = Path.Combine("RMTCAttachments", uniqueFileName).Replace("\\", "/");
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                record.Filename = relativePath;
                record.Id = Id;

                var updateResult = await _rmtcRepository.UpdateAsync(record);
                if (!updateResult.Success)
                    return StatusCode(500, new { message = "Failed to update the record with file info." });

                return Json(new { success = true, message = "done" });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var result = await _rmtcRepository.DeleteAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Error occurred while deleting RMTC record." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetVendors()
        {
            try
            {
                var vendorList = await _rmtcRepository.GetVendorDropdownAsync();
                return Json(vendorList);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetProductCodeSearch(string search)
        {
            var data = await _rmtcRepository.GetCodeSearchAsync(search);
            return Json(data);
        }
    }
}
