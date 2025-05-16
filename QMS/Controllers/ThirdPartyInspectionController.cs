using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.ThirdPartyInspectionRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class ThirdPartyInspectionController : Controller
    {
        private readonly IThirdPartyInspectionRepository _repository;
        private readonly ISystemLogService _systemLogService;

        private readonly string _uploadFolder = "wwwroot/";

        public ThirdPartyInspectionController(
            IThirdPartyInspectionRepository repository,
            ISystemLogService systemLogService)
        {
            _repository = repository;
            _systemLogService = systemLogService;
        }

     

        [HttpGet]
        public async Task<JsonResult> GetAll(DateTime? startDate, DateTime? endDate)
        {
            var list = await _repository.GetListAsync(startDate, endDate);
            return Json(list);
        }

        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            var inspection = await _repository.GetByIdAsync(id);

            if (inspection != null)
            {
                return Json(new { success = true, data = inspection });
            }
            else
            {
                return Json(new { success = false, message = "Inspection not found." });
            }
        }
 
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


       
        [HttpPost]
        public async Task<JsonResult> Create(ThirdPartyInspectionViewModel vm)
        {
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            //    return Json(new { success = false, message = "Validation failed.", errors });
            //}

            try
            {
                bool isUpdate = vm.InspectionID > 0;
                ThirdPartyInspection model;

                if (isUpdate)
                {
                    model = await _repository.GetByIdAsync(vm.InspectionID);
                    if (model == null)
                        return Json(new { success = false, message = "Record not found for update." });

                    // Update existing fields
                    model.InspectionDate = vm.InspectionDate;
                    model.ProjectName = vm.ProjectName;
                    model.InspName = vm.InspName;
                    model.ProductCode = vm.ProductCode;
                    model.ProdDesc = vm.ProdDesc;
                    model.LOTQty = vm.LOTQty;
                    model.ProjectValue = vm.ProjectValue;
                    model.Location = vm.Location;
                    model.Mode = vm.Mode;
                    model.FirstAttempt = vm.FirstAttempt;
                    model.Remark = vm.Remark;
                    model.ActionPlan = vm.ActionPlan;
                    model.MOMDate = vm.MOMDate;
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = HttpContext.Session.GetString("FullName");

                    await _repository.UpdateAsync(model);
                }
                else
                {
                    model = new ThirdPartyInspection
                    {
                        InspectionDate = vm.InspectionDate,
                        ProjectName = vm.ProjectName,
                        InspName = vm.InspName,
                        ProductCode = vm.ProductCode,
                        ProdDesc = vm.ProdDesc,
                        LOTQty = vm.LOTQty,
                        ProjectValue = vm.ProjectValue,
                        Location = vm.Location,
                        Mode = vm.Mode,
                        FirstAttempt = vm.FirstAttempt,
                        Remark = vm.Remark,
                        ActionPlan = vm.ActionPlan,
                        MOMDate = vm.MOMDate,
                        CreatedDate = DateTime.Now,
                        CreatedBy = HttpContext.Session.GetString("FullName")
                    };

                    var result = await _repository.CreateAsync(model, false);

                    if (result == null || !result.Success || result.ObjectId <= 0)
                        return Json(new { success = false, message = "Failed to create third-party inspection." });

                    model.Id = (int)result.ObjectId;
                }

                // Handle attachment upload (shared logic)
                var attachmentPaths = new List<string>();

                if (vm.AttachmentFiles != null && vm.AttachmentFiles.Any())
                {
                    string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ThirdPartyAttachments", model.Id.ToString());
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    const long MaxFileSize = 5 * 1024 * 1024;
                    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

                    foreach (var file in vm.AttachmentFiles)
                    {
                        var ext = Path.GetExtension(file.FileName).ToLower();

                        if (file.Length > MaxFileSize)
                            return Json(new { success = false, message = $"File '{file.FileName}' exceeds the 5MB limit." });

                        if (!allowedExtensions.Contains(ext))
                            return Json(new { success = false, message = $"File type not allowed: {file.FileName}" });

                        string fileName = $"{model.Id}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                        string filePath = Path.Combine(uploadFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await file.CopyToAsync(stream);

                        string relativePath = Path.Combine("ThirdPartyAttachments", model.Id.ToString(), fileName).Replace("\\", "/");
                        attachmentPaths.Add(relativePath);
                    }

                    // Append to existing attachments if updating
                    if (isUpdate && !string.IsNullOrEmpty(model.Attachment))
                        attachmentPaths.Insert(0, model.Attachment); // retain old files

                    model.Attachment = string.Join(";", attachmentPaths.Distinct());
                    model.UpdatedDate = DateTime.Now;
                    model.UpdatedBy = HttpContext.Session.GetString("FullName");

                    await _repository.UpdateAttachmentPath(model);
                }

                return Json(new
                {
                    success = true,
                    message = isUpdate ? "Third-party inspection updated successfully." : "Third-party inspection created successfully.",
                    id = model.Id
                });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "An error occurred while saving the record." });
            }
        }
      

        [HttpPost]
        public async Task<IActionResult> RemoveAttachment(int inspectionId, string filePath)
        {
            var User= HttpContext?.Session.GetString("FullName"); 
            var result = await _repository.RemoveAttachmentAsync(inspectionId, filePath, User);
            if (result.Success)
                return Ok();
            return BadRequest(result.Message);
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var UpdatedBy = HttpContext?.Session.GetString("FullName"); 
                var result = await _repository.DeleteAsync(id, UpdatedBy);
                return Json(result);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Delete failed." });
            }
        }
    }
}
