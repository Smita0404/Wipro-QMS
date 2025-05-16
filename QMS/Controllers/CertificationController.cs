using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.CertificateMasterRepository;
using QMS.Core.Services.SystemLogs;

namespace QMS.Controllers
{
    public class CertificationController : Controller
    {
        private readonly ICertificateMasterRepository _certRepository;
        private readonly ISystemLogService _systemLogService;

        // Constructor to inject the repository and system log service
        public CertificationController(ICertificateMasterRepository certRepository, ISystemLogService systemLogService)
        {
            _certRepository = certRepository;
            _systemLogService = systemLogService;
        }

        // GET request to render the Create view
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST request to handle the creation of a certificate
      
        [HttpPost]
        public async Task<JsonResult> Create(CertificateMaster model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a certificate with the same name already exists
                    bool exists = await _certRepository.CheckDuplicate(model.CertificateName.Trim(), 0);
                    if (exists)
                    {
                        return Json(new { success = false, message = "Certificate already exists." });
                    }

                    // Set audit fields
                    model.CreatedDate = DateTime.Now;
                    model.CreatedBy = HttpContext.Session.GetString("FullName");

                    // Save the certificate
                    var operationResult = await _certRepository.CreateCertificateAsync(model, false);

                    if (operationResult != null && operationResult.Success)
                    {
                        return Json(new { success = true, message = "Certificate created successfully." });
                    }

                    return Json(new { success = false, message = "Failed to create certificate." });
                }
                catch (Exception ex)
                {
                    _systemLogService.WriteLog(ex.Message);
                    return Json(new { success = false, message = "An error occurred while creating the certificate." });
                }
            }

            return Json(new { success = false, message = "Validation failed." });
        }
        [HttpPost]
        public async Task<JsonResult> Update(CertificateMaster model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Use model.CertificateID instead of an undefined 'id' variable
                    var existingCertificate = await _certRepository.GetCertificateByIdAsync(model.Id);
                    if (existingCertificate == null)
                    {
                        return Json(new { success = false, message = "Certificate not found." });
                    }

                    // Check for duplicate name, excluding current record
                    bool exists = await _certRepository.CheckDuplicate(model.CertificateName.Trim(), model.Id);
                    if (exists)
                    {
                        return Json(new { success = false, message = "Certificate with this name already exists." });
                    }

                    // Update properties
                    existingCertificate.CertificateName = model.CertificateName;
                    existingCertificate.UpdatedDate = DateTime.Now;
                    existingCertificate.UpdatedBy = HttpContext.Session.GetString("FullName");

                    // Perform the update
                    var operationResult = await _certRepository.UpdateCertificateAsync(existingCertificate);

                    if (operationResult != null && operationResult.Success)
                    {
                        return Json(new { success = true, message = "Certificate updated successfully." });
                    }

                    return Json(new { success = false, message = "Failed to update certificate." });
                }
                catch (Exception ex)
                {
                    _systemLogService.WriteLog(ex.Message);
                    return Json(new { success = false, message = "An error occurred while updating the certificate." });
                }
            }

            return Json(new { success = false, message = "Validation failed. Please check the input." });
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var UpdatedBy = HttpContext.Session.GetString("FullName");
                var operationResult = await _certRepository.DeleteAsync(id, UpdatedBy);

                return Json(new
                {
                    success = operationResult.Success,
                    message = operationResult.Message
                });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return Json(new
                {
                    success = false,
                    message = "An error occurred while deleting the certificate."
                });
            }
        }

        // GET request to retrieve all certificates
        [HttpGet]
        public async Task<JsonResult> GetAll()
        {
            try
            {
                // Retrieve the list of certificates using the repository
               var certList = await _certRepository.GetCertificatesAsync();

                // Return the list of certificates as JSON
                return Json(new { success = true, data = certList });
            }
            catch (Exception ex)
            {
                // Log the error and return failure response
                _systemLogService.WriteLog(ex.Message);
                return Json(new { success = false, message = "Failed to retrieve certificates." });
            }
        }
    }
}
