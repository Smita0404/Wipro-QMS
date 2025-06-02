using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.COPQComplaintDumpRepository;
using QMS.Core.Services.SystemLogs;
using System;
using System.Threading.Tasks;

namespace QMS.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IComplaintIndentDumpRepository _copqRepository;
        private readonly ISystemLogService _systemLogService;

        public ServiceController(IComplaintIndentDumpRepository copqRepository, ISystemLogService systemLogService)
        {
            _copqRepository = copqRepository;
            _systemLogService = systemLogService;
        }

        // Main page/view
        public IActionResult Service()
        {
            return View();
        }


        //// ----------------- Complaint Dump ------------------- ////

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
        [Route("Service/CreateAsync")]
        public async Task<JsonResult> CreateAsync([FromBody] ComplaintDump_Service model)
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
        [Route("Service/UpdateAsync")]
        public async Task<JsonResult> UpdateAsync([FromBody] ComplaintDump_Service model)
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


        [HttpPost]
        public async Task<IActionResult> UploadComplaintDumpExcel(IFormFile file, string fileName, string uploadDate, int recordCount)
        {
            var prRecordsToAdd = new List<ComplaintViewModel>();

            try
            {
                var uploadedBy = HttpContext.Session.GetString("FullName");
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rowCount = worksheet.RowsUsed().Count();

                for (int row = 2; row <= rowCount; row++)
                {
                    var model = new ComplaintViewModel
                    {
                        CCCNDate = worksheet.Cell(row, 1).TryGetValue(out DateTime ccnDate) ? ccnDate : null,
                        ReportedBy = worksheet.Cell(row, 2).GetString().Trim(),
                        CLocation = worksheet.Cell(row, 3).GetString().Trim(),
                        CustName = worksheet.Cell(row, 4).GetString().Trim(),
                        DealerName = worksheet.Cell(row, 5).GetString().Trim(),
                        CDescription = worksheet.Cell(row, 6).GetString().Trim(),
                        CStatus = worksheet.Cell(row, 7).GetString().Trim(),
                        Completion = worksheet.Cell(row, 8).GetString().Trim(),
                        Remarks = worksheet.Cell(row, 9).GetString().Trim(),
                        CreatedBy = uploadedBy,
                        CreatedDate = DateTime.Now,
                    };

                    prRecordsToAdd.Add(model);
                }

                var importResult = await _copqRepository.BulkCreateAsync(prRecordsToAdd, fileName, uploadedBy, "ComplaintDump");

                // If there are failed records, return file
                if (importResult.FailedRecords.Any())
                {
                    using var failStream = new MemoryStream();
                    using var failWb = new XLWorkbook();
                    var failSheet = failWb.Worksheets.Add("Failed Records");

                    failSheet.Cell(1, 1).Value = "CCNDate";
                    failSheet.Cell(1, 2).Value = "CustName";
                    failSheet.Cell(1, 3).Value = "Reason";

                    int i = 2;
                    foreach (var fail in importResult.FailedRecords)
                    {
                        failSheet.Cell(i, 1).Value = fail.Record.CCCNDate;
                        failSheet.Cell(i, 2).Value = fail.Record.CustName;
                        failSheet.Cell(i, 3).Value = fail.Reason;
                        i++;
                    }

                    failWb.SaveAs(failStream);
                    failStream.Position = 0;

                    var failedFileName = $"Failed_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    Response.Headers["Content-Disposition"] = $"attachment; filename={failedFileName}";
                    return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    //string logFileName = $"FailedOpenPO_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    //return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", logFileName);
                }

                return Json(new
                {
                    success = true,
                    message = $"Import completed. Total: {recordCount}, Saved: {prRecordsToAdd.Count}, Duplicates: 0"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Import failed: " + ex.Message
                });
            }
        }


        //// ----------------- Complaint Dump ------------------- ////


        //// ----------------- Po List ------------------- ////

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
        public async Task<JsonResult> CreatePO([FromBody] PendingPo_Service model)
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
        public async Task<JsonResult> UpdatePO([FromBody] PendingPo_Service model)
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

        [HttpPost]
        public async Task<IActionResult> UploadPoDumpExcel(IFormFile file, string fileName, string uploadDate, int recordCount)
        {
            var prRecordsToAdd = new List<PendingPoViewModel>();

            try
            {
                var uploadedBy = HttpContext.Session.GetString("FullName");
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rowCount = worksheet.RowsUsed().Count();

                for (int row = 2; row <= rowCount; row++)
                {
                    var model = new PendingPoViewModel
                    {
                        Vendor = worksheet.Cell(row, 1).GetString().Trim(),
                        Material = worksheet.Cell(row, 2).GetString().Trim(),
                        ReferenceNo = worksheet.Cell(row, 3).GetString().Trim(),
                        PONo = worksheet.Cell(row, 4).GetString().Trim(),
                        PODate = worksheet.Cell(row, 5).TryGetValue(out DateTime poDate) ? poDate : null,
                        PRNo = worksheet.Cell(row, 6).GetString().Trim(),
                        BatchNo = worksheet.Cell(row, 7).GetString().Trim(),
                        POQty = worksheet.Cell(row, 8).GetString().Trim(),
                        BalanceQty = worksheet.Cell(row, 9).GetString().Trim(),
                        Destination = worksheet.Cell(row, 10).GetString().Trim(),
                        BalanceValue = worksheet.Cell(row, 11).GetString().Trim(),
                        CreatedBy = uploadedBy,
                        CreatedDate = DateTime.Now,
                    };

                    prRecordsToAdd.Add(model);
                }

                var importResult = await _copqRepository.BulkCreatePoAsync(prRecordsToAdd, fileName, uploadedBy, "PoDump");

                // If there are failed records, return file
                if (importResult.FailedRecords.Any())
                {
                    using var failStream = new MemoryStream();
                    using var failWb = new XLWorkbook();
                    var failSheet = failWb.Worksheets.Add("Failed Records");

                    failSheet.Cell(1, 1).Value = "Vendor";
                    failSheet.Cell(1, 2).Value = "PONo";
                    failSheet.Cell(1, 3).Value = "Reason";

                    int i = 2;
                    foreach (var fail in importResult.FailedRecords)
                    {
                        failSheet.Cell(i, 1).Value = fail.Record.Vendor;
                        failSheet.Cell(i, 2).Value = fail.Record.PONo;
                        failSheet.Cell(i, 3).Value = fail.Reason;
                        i++;
                    }

                    failWb.SaveAs(failStream);
                    failStream.Position = 0;

                    var failedFileName = $"Failed_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    Response.Headers["Content-Disposition"] = $"attachment; filename={failedFileName}";
                    return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    //string logFileName = $"FailedOpenPO_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    //return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", logFileName);
                }

                return Json(new
                {
                    success = true,
                    message = $"Import completed. Total: {recordCount}, Saved: {prRecordsToAdd.Count}, Duplicates: 0"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Import failed: " + ex.Message
                });
            }
        }


        //// ----------------- Po List ------------------- ////

        //// ----------------- Indant Dump ------------------- ////

        // Get PO list with optional date range
        [HttpGet]
        public async Task<JsonResult> GetAllIndent(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var list = await _copqRepository.GetIndentListAsync(startDate, endDate);
                return Json(new { success = true, data = list });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetAll Indent Error: {ex.Message}");
                return Json(new { success = false, message = "Failed to load Indent list." });
            }
        }

        // GET: /PO/GetById/5
        [HttpGet]
        public async Task<JsonResult> GetByIdIndent(int id)
        {
            try
            {
                var po = await _copqRepository.GetIndentByIdAsync(id);
                if (po == null)
                    return Json(new { success = false, message = "Indent not found." });

                return Json(new { success = true, data = po });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetById PO Error: {ex.Message}");
                return Json(new { success = false, message = "Failed to get PO details." });
            }
        }
        [HttpPost]
        public async Task<JsonResult> CreateIndent([FromBody] IndentDump_Service model)
        {
            try
            {
                if (model == null)
                    return Json(new { success = false, message = "Invalid Indent data." });

                model.CreatedDate = DateTime.Now;
                model.CreatedBy = HttpContext.Session.GetString("FullName") ?? "System";
                model.Deleted = false;

                var result = await _copqRepository.CreateIndentAsync(model);
                if (result.Success)
                    return Json(new { success = true, message = "Indent Dump created successfully." });

                return Json(new { success = false, message = result.Message ?? "Failed to create Indent." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Create PO Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while creating Indent." });
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateIndent([FromBody] IndentDump_Service model)
        {
            try
            {
                if (model == null || model.Id <= 0)
                    return Json(new { success = false, message = "Invalid Indent update data." });

                model.UpdatedDate = DateTime.Now;
                model.UpdatedBy = HttpContext.Session.GetString("FullName") ?? "System";

                var result = await _copqRepository.UpdateIndentAsync(model);
                if (result.Success)
                    return Json(new { success = true, message = "Indent Dump updated successfully." });

                return Json(new { success = false, message = result.Message ?? "Failed to update Indent." });
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Update PO Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while updating Indent." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> IndentDelete(int id)
        {
            try
            {
                var result = await _copqRepository.DeleteIndentAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Delete Indent Error: {ex.Message}");
                return Json(new { success = false, message = "Error occurred while deleting Indent." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadIndentDumpExcel(IFormFile file, string fileName, string uploadDate, int recordCount)
        {
            var prRecordsToAdd = new List<IndentDumpViewModel>();

            try
            {
                var uploadedBy = HttpContext.Session.GetString("FullName");
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);
                var rowCount = worksheet.RowsUsed().Count();

                for (int row = 2; row <= rowCount; row++)
                {
                    var model = new IndentDumpViewModel
                    {
                        Indent_No = worksheet.Cell(row, 1).GetString().Trim(),
                        Indent_Date = worksheet.Cell(row, 2).TryGetValue(out DateTime inDate) ? inDate : null,
                        Business_Unit = worksheet.Cell(row, 3).GetString().Trim(),
                        Vertical = worksheet.Cell(row, 4).GetString().Trim(),
                        Branch = worksheet.Cell(row, 5).GetString().Trim(),
                        Indent_Status = worksheet.Cell(row, 6).GetString().Trim(),
                        End_Cust_Name = worksheet.Cell(row, 7).GetString().Trim(),
                        Complaint_Id = worksheet.Cell(row, 8).GetString().Trim(),
                        Customer_Code = worksheet.Cell(row, 9).GetString().Trim(),
                        Customer_Name = worksheet.Cell(row, 10).GetString().Trim(),
                        Bill_Req_Date = worksheet.Cell(row, 11).TryGetValue(out DateTime billDate) ? billDate : null,
                        Created_By = worksheet.Cell(row, 12).GetString().Trim(),
                        Wipro_Commit_Date = worksheet.Cell(row, 13).GetString().Trim(),
                        Material_No = worksheet.Cell(row, 14).GetString().Trim(),
                        Item_Description = worksheet.Cell(row, 15).GetString().Trim(),
                        Quantity = worksheet.Cell(row, 16).GetValue<int?>(),
                        Price = worksheet.Cell(row, 17).GetString().Trim(),
                        Final_Price = worksheet.Cell(row, 18).GetString().Trim(),
                        SapSoNo = worksheet.Cell(row, 19).GetString().Trim(),
                        CreateSoQty = worksheet.Cell(row, 20).GetValue<int?>(),
                        Inv_Qty = worksheet.Cell(row, 21).GetValue<int?>(),
                        Inv_Value = worksheet.Cell(row, 22).GetString().Trim(),
                        WiproCatelog_No = worksheet.Cell(row, 23).GetString().Trim(),
                        Batch_Code = worksheet.Cell(row, 24).GetString().Trim(),
                        Batch_Date = worksheet.Cell(row, 25).TryGetValue(out DateTime btDate) ? btDate : null,
                        Main_Prodcode = worksheet.Cell(row, 26).GetString().Trim(),
                        User_Name = worksheet.Cell(row, 27).GetString().Trim(),
                        CreatedBy = uploadedBy,
                        CreatedDate = DateTime.Now,
                    };

                    prRecordsToAdd.Add(model);
                }

                var importResult = await _copqRepository.BulkCreateIndentAsync(prRecordsToAdd, fileName, uploadedBy, "IndentDump");

                // If there are failed records, return file
                if (importResult.FailedRecords.Any())
                {
                    using var failStream = new MemoryStream();
                    using var failWb = new XLWorkbook();
                    var failSheet = failWb.Worksheets.Add("Failed Records");

                    failSheet.Cell(1, 1).Value = "Indent_No";
                    failSheet.Cell(1, 2).Value = "Customer_Code";
                    failSheet.Cell(1, 3).Value = "Reason";

                    int i = 2;
                    foreach (var fail in importResult.FailedRecords)
                    {
                        failSheet.Cell(i, 1).Value = fail.Record.Indent_No;
                        failSheet.Cell(i, 2).Value = fail.Record.Customer_Code;
                        failSheet.Cell(i, 3).Value = fail.Reason;
                        i++;
                    }

                    failWb.SaveAs(failStream);
                    failStream.Position = 0;

                    var failedFileName = $"Failed_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    Response.Headers["Content-Disposition"] = $"attachment; filename={failedFileName}";
                    return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    //string logFileName = $"FailedOpenPO_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    //return File(failStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", logFileName);
                }

                return Json(new
                {
                    success = true,
                    message = $"Import completed. Total: {recordCount}, Saved: {prRecordsToAdd.Count}, Duplicates: 0"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Import failed: " + ex.Message
                });
            }
        }


        //// ----------------- Po List ------------------- ////


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
