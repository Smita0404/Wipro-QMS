using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;


namespace QMS.Core.Repositories.COPQComplaintDumpRepository
{
    public class ComplaintIndentDumpRepository : SqlTableRepository, IComplaintIndentDumpRepository
    {
        private new readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public ComplaintIndentDumpRepository(QMSDbContext dbContext, ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<ComplaintViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.COPQComplaintDump
                    .FromSqlRaw("EXEC sp_Get_COPQComplaintDump")
                    .ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result
                        .Where(x => x.CCCNDate.HasValue &&
                                    x.CCCNDate.Value.Date >= startDate.Value.Date &&
                                    x.CCCNDate.Value.Date <= endDate.Value.Date)
                        .ToList();
                }

                return result.Select(x => new ComplaintViewModel
                {
                    Id = x.Id,
                    Deleted = x.Deleted,
                    CCCNDate = x.CCCNDate,
                    ReportedBy = x.ReportedBy,
                    CLocation = x.CLocation,
                    CustName = x.CustName,
                    DealerName = x.DealerName,
                    CDescription = x.CDescription,
                    CStatus = x.CStatus,
                    Completion = x.Completion,
                    Remarks = x.Remarks,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(ComplaintDump_Service record, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CCCNDate", record.CCCNDate ?? (object)DBNull.Value),
                    new SqlParameter("@ReportedBy", record.ReportedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CLocation", record.CLocation ?? (object)DBNull.Value),
                    new SqlParameter("@CustName", record.CustName ?? (object)DBNull.Value),
                    new SqlParameter("@DealerName", record.DealerName ?? (object)DBNull.Value),
                    new SqlParameter("@CDescription", record.CDescription ?? (object)DBNull.Value),
                    new SqlParameter("@CStatus", record.CStatus ?? (object)DBNull.Value),
                    new SqlParameter("@Completion", record.Completion ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", record.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@CreatedBy", record.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", record.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Insert_COPQComplaintDump @CCCNDate, @ReportedBy, @CLocation, @CustName, @DealerName, @CDescription, @CStatus, @Completion, @Remarks, @CreatedDate, @CreatedBy, @IsDeleted",
                    parameters
                );

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> UpdateAsync(ComplaintDump_Service record, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", record.Id),
                    new SqlParameter("@CCCNDate", record.CCCNDate ?? (object)DBNull.Value),
                    new SqlParameter("@ReportedBy", record.ReportedBy ?? (object)DBNull.Value),
                    new SqlParameter("@CLocation", record.CLocation ?? (object)DBNull.Value),
                    new SqlParameter("@CustName", record.CustName ?? (object)DBNull.Value),
                    new SqlParameter("@DealerName", record.DealerName ?? (object)DBNull.Value),
                    new SqlParameter("@CDescription", record.CDescription ?? (object)DBNull.Value),
                    new SqlParameter("@CStatus", record.CStatus ?? (object)DBNull.Value),
                    new SqlParameter("@Completion", record.Completion ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", record.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", record.UpdatedBy ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Update_COPQComplaintDump @Id, @CCCNDate, @ReportedBy, @CLocation, @CustName, @DealerName, @CDescription, @CStatus, @Completion, @Remarks, @UpdatedBy",
                    parameters
                );

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                return await base.DeleteAsync<ComplaintDump_Service>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<ComplaintViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@Id", id) };

                var result = await _dbContext.COPQComplaintDump
                    .FromSqlRaw("EXEC sp_Get_COPQComplaintDump_ById @Id", parameters)
                    .ToListAsync();

                return result.Select(x => new ComplaintViewModel
                {
                    Id = x.Id,
                    Deleted = x.Deleted,
                    CCCNDate = x.CCCNDate,
                    ReportedBy = x.ReportedBy,
                    CLocation = x.CLocation,
                    CustName = x.CustName,
                    DealerName = x.DealerName,
                    CDescription = x.CDescription,
                    CStatus = x.CStatus,
                    Completion = x.Completion,
                    Remarks = x.Remarks,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<BulkCreateResult> BulkCreateAsync(List<ComplaintViewModel> listOfData, string fileName, string uploadedBy, string recordType)
        {
            var result = new BulkCreateResult();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var seenKeys = new HashSet<string>(); // for tracking in-batch duplicates

                foreach (var item in listOfData)
                {
                    item.CCCNDate = item.CCCNDate;
                    item.CustName = item.CustName?.Trim();

                    var compositeKey = $"{item.CCCNDate}|{item.CustName}";

                    if (string.IsNullOrWhiteSpace(item.CCCNDate.ToString()) || string.IsNullOrWhiteSpace(item.CustName))
                    {
                        result.FailedRecords.Add((item, "Missing Date or Custname"));
                        continue;
                    }

                    // Check if this key combination has already been seen in the batch
                    if (seenKeys.Contains(compositeKey))
                    {
                        result.FailedRecords.Add((item, "Duplicate in uploaded file"));
                        continue;
                    }

                    seenKeys.Add(compositeKey); // Mark this combination as seen

                    // Now check against database
                    bool existsInDb = await _dbContext.COPQComplaintDump
                        .AnyAsync(x => x.CCCNDate == item.CCCNDate && x.CustName == item.CustName);

                    if (existsInDb)
                    {
                        result.FailedRecords.Add((item, "Duplicate in database"));
                        continue;
                    }

                    // Passed all checks — add to DB
                    var entity = new ComplaintDump_Service
                    {
                        CCCNDate = item.CCCNDate,
                        ReportedBy = item.ReportedBy,
                        CLocation = item.CLocation,
                        CustName = item.CustName,
                        DealerName = item.DealerName,
                        CDescription = item.CDescription,
                        CStatus = item.CStatus,
                        Completion = item.Completion,
                        Remarks = item.Remarks,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    };

                    _dbContext.COPQComplaintDump.Add(entity);
                }

                await _dbContext.SaveChangesAsync();

                // Log the upload
                var importLog = new FailedRecord_Log
                {
                    FileName = fileName,
                    TotalRecords = listOfData.Count,
                    ImportedRecords = listOfData.Count - result.FailedRecords.Count,
                    FailedRecords = result.FailedRecords.Count,
                    RecordType = recordType,
                    UploadedBy = uploadedBy,
                    UploadedAt = DateTime.Now
                };

                _dbContext.FailedRecord_Log.Add(importLog);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Result = new OperationResult
                {
                    Success = true,
                    Message = result.FailedRecords.Any()
                        ? "Import completed with some skipped records."
                        : "All records imported successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Result = new OperationResult
                {
                    Success = false,
                    Message = "Error during import: " + ex.Message
                };
            }

            return result;
        }


        //// ----------------- Complaint Dump ------------------- ////



        //// ----------------- Po List ------------------- ////
        public async Task<List<PendingPoViewModel>> GetPOListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.PODetails
                    .FromSqlRaw("EXEC sp_Get_POList")
                    .ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result.Where(x =>
                        x.CreatedDate.HasValue &&
                        x.CreatedDate.Value.Date >= startDate.Value.Date &&
                        x.CreatedDate.Value.Date <= endDate.Value.Date
                    ).ToList();
                }

                return result.Select(x => new PendingPoViewModel
                {
                    Id = x.Id,
                    Vendor = x.Vendor,
                    Material = x.Material,
                    ReferenceNo = x.ReferenceNo,
                    PONo = x.PONo,
                    PODate = x.PODate,
                    PRNo = x.PRNo,
                    BatchNo = x.BatchNo,
                    POQty = x.POQty,
                    BalanceQty = x.BalanceQty,
                    Destination = x.Destination,
                    BalanceValue = x.BalanceValue,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetPOListAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<PendingPoViewModel?> GetPOByIdAsync(int id)
        {
            try
            {
                var param = new SqlParameter("@Id", id);
                var result = await _dbContext.PODetails
                    .FromSqlRaw("EXEC sp_Get_POById @Id", param)
                    .ToListAsync();

                return result.Select(x => new PendingPoViewModel
                {
                    Id = x.Id,
                    Vendor = x.Vendor,
                    Material = x.Material,
                    ReferenceNo = x.ReferenceNo,
                    PONo = x.PONo,
                    PODate = x.PODate,
                    PRNo = x.PRNo,
                    BatchNo = x.BatchNo,
                    POQty = x.POQty,
                    BalanceQty = x.BalanceQty,
                    Destination = x.Destination,
                    BalanceValue = x.BalanceValue,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetPOByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> CreatePOAsync(PendingPo_Service podetail, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Vendor", podetail.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@Material", podetail.Material ?? (object)DBNull.Value),
                    new SqlParameter("@ReferenceNo", podetail.ReferenceNo ?? (object)DBNull.Value),
                    new SqlParameter("@PONo", podetail.PONo ?? (object)DBNull.Value),
                    new SqlParameter("@PODate", podetail.PODate ?? (object)DBNull.Value),
                    new SqlParameter("@PRNo", podetail.PRNo ?? (object)DBNull.Value),
                    new SqlParameter("@BatchNo", podetail.BatchNo ?? (object)DBNull.Value),
                    new SqlParameter("@POQty", podetail.POQty ?? (object)DBNull.Value),
                    new SqlParameter("@BalanceQty", podetail.BalanceQty ?? (object)DBNull.Value),
                    new SqlParameter("@Destination", podetail.Destination ?? (object)DBNull.Value),
                    new SqlParameter("@BalanceValue", podetail.BalanceValue ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedDate", DateTime.Now),
                    new SqlParameter("@CreatedBy", podetail.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", podetail.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_Insert_PO 
                        @Vendor, @Material, @ReferenceNo, @PONo, @PODate, @PRNo, @BatchNo, 
                        @POQty, @BalanceQty, @Destination, @BalanceValue, @CreatedDate, @CreatedBy, @IsDeleted",
                    parameters
                );

                // Optional: if you want to fetch and return the created record, implement that here.

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"CreatePOAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> UpdatePOAsync(PendingPo_Service podetail, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", podetail.Id),
                    new SqlParameter("@Vendor", podetail.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@Material", podetail.Material ?? (object)DBNull.Value),
                    new SqlParameter("@ReferenceNo", podetail.ReferenceNo ?? (object)DBNull.Value),
                    new SqlParameter("@PONo", podetail.PONo ?? (object)DBNull.Value),
                    new SqlParameter("@PODate", podetail.PODate ?? (object)DBNull.Value),
                    new SqlParameter("@PRNo", podetail.PRNo ?? (object)DBNull.Value),
                    new SqlParameter("@BatchNo", podetail.BatchNo ?? (object)DBNull.Value),
                    new SqlParameter("@POQty", podetail.POQty ?? (object)DBNull.Value),
                    new SqlParameter("@BalanceQty", podetail.BalanceQty ?? (object)DBNull.Value),
                    new SqlParameter("@Destination", podetail.Destination ?? (object)DBNull.Value),
                    new SqlParameter("@BalanceValue", podetail.BalanceValue ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedDate", DateTime.Now),
                    new SqlParameter("@UpdatedBy", podetail.UpdatedBy ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_Update_PO 
                        @Id, @Vendor, @Material, @ReferenceNo, @PONo, @PODate, @PRNo, @BatchNo, 
                        @POQty, @BalanceQty, @Destination, @BalanceValue, @UpdatedDate, @UpdatedBy",
                    parameters
                );

                // Optional: fetch updated record here if returnUpdatedRecord == true

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"UpdatePOAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> DeletePOAsync(int id)
        {
            try
            {
                return await base.DeleteAsync<PendingPo_Service>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"DeletePOAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<BulkCreatePOResult> BulkCreatePoAsync(List<PendingPoViewModel> listOfData, string fileName, string uploadedBy, string recordType)
        {
            var result = new BulkCreatePOResult();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var seenKeys = new HashSet<string>(); // for tracking in-batch duplicates

                foreach (var item in listOfData)
                {
                    item.Vendor = item.Vendor?.Trim();
                    item.PONo = item.PONo?.Trim();

                    var compositeKey = $"{item.Vendor}|{item.PONo}";

                    if (string.IsNullOrWhiteSpace(item.Vendor) || string.IsNullOrWhiteSpace(item.PONo))
                    {
                        result.FailedRecords.Add((item, "Missing Vendor or Po. No"));
                        continue;
                    }

                    // Check if this key combination has already been seen in the batch
                    if (seenKeys.Contains(compositeKey))
                    {
                        result.FailedRecords.Add((item, "Duplicate in uploaded file"));
                        continue;
                    }

                    seenKeys.Add(compositeKey); // Mark this combination as seen

                    // Now check against database
                    bool existsInDb = await _dbContext.PODetails
                        .AnyAsync(x => x.Vendor == item.Vendor && x.PONo == item.PONo);

                    if (existsInDb)
                    {
                        result.FailedRecords.Add((item, "Duplicate in database"));
                        continue;
                    }

                    // Passed all checks — add to DB
                    var entity = new PendingPo_Service
                    {
                        Vendor = item.Vendor,
                        Material = item.Material,
                        ReferenceNo = item.ReferenceNo,
                        PONo = item.PONo,
                        PODate = item.PODate,
                        PRNo = item.PRNo,
                        BatchNo = item.BatchNo,
                        POQty = item.POQty,
                        BalanceQty = item.BalanceQty,
                        Destination = item.Destination,
                        BalanceValue = item.BalanceValue,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    };

                    _dbContext.PODetails.Add(entity);
                }

                await _dbContext.SaveChangesAsync();

                // Log the upload
                var importLog = new FailedRecord_Log
                {
                    FileName = fileName,
                    TotalRecords = listOfData.Count,
                    ImportedRecords = listOfData.Count - result.FailedRecords.Count,
                    FailedRecords = result.FailedRecords.Count,
                    RecordType = recordType,
                    UploadedBy = uploadedBy,
                    UploadedAt = DateTime.Now
                };

                _dbContext.FailedRecord_Log.Add(importLog);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Result = new OperationResult
                {
                    Success = true,
                    Message = result.FailedRecords.Any()
                        ? "Import completed with some skipped records."
                        : "All records imported successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Result = new OperationResult
                {
                    Success = false,
                    Message = "Error during import: " + ex.Message
                };
            }

            return result;
        }

        //// ----------------- Po List ------------------- ////


        //// ----------------- Indent Dump ------------------- ////
        public async Task<List<IndentDumpViewModel>> GetIndentListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.IndentDump
                    .FromSqlRaw("EXEC sp_Get_IndentDump")
                    .ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result.Where(x =>
                        x.CreatedDate.HasValue &&
                        x.CreatedDate.Value.Date >= startDate.Value.Date &&
                        x.CreatedDate.Value.Date <= endDate.Value.Date
                    ).ToList();
                }

                return result.Select(x => new IndentDumpViewModel
                {
                    Id = x.Id,
                    Indent_No = x.Indent_No,
                    Indent_Date = x.Indent_Date,
                    Business_Unit = x.Business_Unit,
                    Vertical = x.Vertical,
                    Branch = x.Branch,
                    Indent_Status = x.Indent_Status,
                    End_Cust_Name = x.End_Cust_Name,
                    Complaint_Id = x.Complaint_Id,
                    Customer_Code = x.Customer_Code,
                    Customer_Name = x.Customer_Name,
                    Bill_Req_Date = x.Bill_Req_Date,
                    Created_By = x.Created_By,
                    Wipro_Commit_Date = x.Wipro_Commit_Date,
                    Material_No = x.Material_No,
                    Item_Description = x.Item_Description,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Final_Price = x.Final_Price,
                    SapSoNo = x.SapSoNo,
                    CreateSoQty = x.CreateSoQty,
                    Inv_Qty = x.Inv_Qty,
                    Inv_Value = x.Inv_Value,
                    WiproCatelog_No = x.WiproCatelog_No,
                    Batch_Code = x.Batch_Code,
                    Batch_Date = x.Batch_Date,
                    Main_Prodcode = x.Main_Prodcode,
                    User_Name = x.User_Name,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetPOListAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<IndentDumpViewModel?> GetIndentByIdAsync(int id)
        {
            try
            {
                var param = new SqlParameter("@Indent_Id", id);
                var result = await _dbContext.IndentDump
                    .FromSqlRaw("EXEC sp_Get_Indent_ById @Indent_Id", param)
                    .ToListAsync();

                return result.Select(x => new IndentDumpViewModel
                {
                    Id = x.Id,
                    Indent_No = x.Indent_No,
                    Indent_Date = x.Indent_Date,
                    Business_Unit = x.Business_Unit,
                    Vertical = x.Vertical,
                    Branch = x.Branch,
                    Indent_Status = x.Indent_Status,
                    End_Cust_Name = x.End_Cust_Name,
                    Complaint_Id = x.Complaint_Id,
                    Customer_Code = x.Customer_Code,
                    Customer_Name = x.Customer_Name,
                    Bill_Req_Date = x.Bill_Req_Date,
                    Created_By = x.Created_By,
                    Wipro_Commit_Date = x.Wipro_Commit_Date,
                    Material_No = x.Material_No,
                    Item_Description = x.Item_Description,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Final_Price = x.Final_Price,
                    SapSoNo = x.SapSoNo,
                    CreateSoQty = x.CreateSoQty,
                    Inv_Qty = x.Inv_Qty,
                    Inv_Value = x.Inv_Value,
                    WiproCatelog_No = x.WiproCatelog_No,
                    Batch_Code = x.Batch_Code,
                    Batch_Date = x.Batch_Date,
                    Main_Prodcode = x.Main_Prodcode,
                    User_Name = x.User_Name,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"GetPOByIdAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> CreateIndentAsync(IndentDump_Service indentdetail, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Indent_No", indentdetail.Indent_No ?? (object)DBNull.Value),
                    new SqlParameter("@Indent_Date", indentdetail.Indent_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Business_Unit", indentdetail.Business_Unit ?? (object)DBNull.Value),
                    new SqlParameter("@Vertical", indentdetail.Vertical ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", indentdetail.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@Indent_Status", indentdetail.Indent_Status ?? (object)DBNull.Value),
                    new SqlParameter("@End_Cust_Name", indentdetail.End_Cust_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Complaint_Id", indentdetail.Complaint_Id ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Code", indentdetail.Customer_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Name", indentdetail.Customer_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Bill_Req_Date", indentdetail.Bill_Req_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Created_By", indentdetail.Created_By ?? (object)DBNull.Value),
                    new SqlParameter("@Wipro_Commit_Date", indentdetail.Wipro_Commit_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Material_No", indentdetail.Material_No ?? (object)DBNull.Value),
                    new SqlParameter("@Item_Description", indentdetail.Item_Description ?? (object)DBNull.Value),
                    new SqlParameter("@Quantity", indentdetail.Quantity ?? (object)DBNull.Value),
                    new SqlParameter("@Price", indentdetail.Price ?? (object)DBNull.Value),
                    new SqlParameter("@Final_Price", indentdetail.Final_Price ?? (object)DBNull.Value),
                    new SqlParameter("@SapSoNo", indentdetail.SapSoNo ?? (object)DBNull.Value),
                    new SqlParameter("@CreateSoQty", indentdetail.CreateSoQty ?? (object)DBNull.Value),
                    new SqlParameter("@Inv_Qty", indentdetail.Inv_Qty ?? (object)DBNull.Value),
                    new SqlParameter("@Inv_Value", indentdetail.Inv_Value ?? (object)DBNull.Value),
                    new SqlParameter("@WiproCatelog_No", indentdetail.WiproCatelog_No ?? (object)DBNull.Value),
                    new SqlParameter("@Batch_Code", indentdetail.Batch_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Batch_Date", indentdetail.Batch_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Main_Prodcode", indentdetail.Main_Prodcode ?? (object)DBNull.Value),
                    new SqlParameter("@User_Name", indentdetail.User_Name ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", indentdetail.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", indentdetail.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_Insert_IndentDump 
                        @Indent_No, @Indent_Date, @Business_Unit, @Vertical, @Branch, @Indent_Status, @End_Cust_Name,@Complaint_Id,@Customer_Code,@Customer_Name,@Bill_Req_Date,@Created_By,
                        @Wipro_Commit_Date, @Material_No, @Item_Description, @Quantity, @Price, @Final_Price, @SapSoNo, @CreateSoQty,@Inv_Qty,@Inv_Value,@WiproCatelog_No,@Batch_Code,
                        @Batch_Date, @Main_Prodcode, @User_Name, @CreatedBy, @IsDeleted",
                    parameters
                );

                // Optional: if you want to fetch and return the created record, implement that here.

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"CreatePOAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> UpdateIndentAsync(IndentDump_Service indentdetail, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", indentdetail.Id),
                    new SqlParameter("@Indent_No", indentdetail.Indent_No ?? (object)DBNull.Value),
                    new SqlParameter("@Indent_Date", indentdetail.Indent_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Business_Unit", indentdetail.Business_Unit ?? (object)DBNull.Value),
                    new SqlParameter("@Vertical", indentdetail.Vertical ?? (object)DBNull.Value),
                    new SqlParameter("@Branch", indentdetail.Branch ?? (object)DBNull.Value),
                    new SqlParameter("@Indent_Status", indentdetail.Indent_Status ?? (object)DBNull.Value),
                    new SqlParameter("@End_Cust_Name", indentdetail.End_Cust_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Complaint_Id", indentdetail.Complaint_Id ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Code", indentdetail.Customer_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Name", indentdetail.Customer_Name ?? (object)DBNull.Value),
                    new SqlParameter("@Bill_Req_Date", indentdetail.Bill_Req_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Created_By", indentdetail.Created_By ?? (object)DBNull.Value),
                    new SqlParameter("@Wipro_Commit_Date", indentdetail.Wipro_Commit_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Material_No", indentdetail.Material_No ?? (object)DBNull.Value),
                    new SqlParameter("@Item_Description", indentdetail.Item_Description ?? (object)DBNull.Value),
                    new SqlParameter("@Quantity", indentdetail.Quantity ?? (object)DBNull.Value),
                    new SqlParameter("@Price", indentdetail.Price ?? (object)DBNull.Value),
                    new SqlParameter("@Final_Price", indentdetail.Final_Price ?? (object)DBNull.Value),
                    new SqlParameter("@SapSoNo", indentdetail.SapSoNo ?? (object)DBNull.Value),
                    new SqlParameter("@CreateSoQty", indentdetail.CreateSoQty ?? (object)DBNull.Value),
                    new SqlParameter("@Inv_Qty", indentdetail.Inv_Qty ?? (object)DBNull.Value),
                    new SqlParameter("@Inv_Value", indentdetail.Inv_Value ?? (object)DBNull.Value),
                    new SqlParameter("@WiproCatelog_No", indentdetail.WiproCatelog_No ?? (object)DBNull.Value),
                    new SqlParameter("@Batch_Code", indentdetail.Batch_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Batch_Date", indentdetail.Batch_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Main_Prodcode", indentdetail.Main_Prodcode ?? (object)DBNull.Value),
                    new SqlParameter("@User_Name", indentdetail.User_Name ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", indentdetail.UpdatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", indentdetail.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_Update_Indent 
                        @Id,  @Indent_No, @Indent_Date, @Business_Unit, @Vertical, @Branch, @Indent_Status, @End_Cust_Name,@Complaint_Id,@Customer_Code,@Customer_Name,@Bill_Req_Date,@Created_By,
                        @Wipro_Commit_Date, @Material_No, @Item_Description, @Quantity, @Price, @Final_Price, @SapSoNo, @CreateSoQty,@Inv_Qty,@Inv_Value,@WiproCatelog_No,@Batch_Code,
                        @Batch_Date, @Main_Prodcode, @User_Name, @UpdatedBy, @IsDeleted",
                    parameters
                );

                // Optional: fetch updated record here if returnUpdatedRecord == true

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"UpdatePOAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<OperationResult> DeleteIndentAsync(int id)
        {
            try
            {
                return await base.DeleteAsync<IndentDump_Service>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"DeletePOAsync: {ex.Message}");
                throw;
            }
        }


        public async Task<BulkCreateIndentResult> BulkCreateIndentAsync(List<IndentDumpViewModel> listOfData, string fileName, string uploadedBy, string recordType)
        {
            var result = new BulkCreateIndentResult();
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var seenKeys = new HashSet<string>(); // for tracking in-batch duplicates

                foreach (var item in listOfData)
                {
                    item.Indent_No = item.Indent_No?.Trim();
                    item.Customer_Code = item.Customer_Code?.Trim();

                    var compositeKey = $"{item.Indent_No}|{item.Customer_Code}";

                    if (string.IsNullOrWhiteSpace(item.Indent_No) || string.IsNullOrWhiteSpace(item.Customer_Code))
                    {
                        result.FailedRecords.Add((item, "Missing Indent No. or Customer Code"));
                        continue;
                    }

                    // Check if this key combination has already been seen in the batch
                    if (seenKeys.Contains(compositeKey))
                    {
                        result.FailedRecords.Add((item, "Duplicate in uploaded file"));
                        continue;
                    }

                    seenKeys.Add(compositeKey); // Mark this combination as seen

                    // Now check against database
                    bool existsInDb = await _dbContext.IndentDump
                        .AnyAsync(x => x.Indent_No == item.Indent_No && x.Customer_Code == item.Customer_Code);

                    if (existsInDb)
                    {
                        result.FailedRecords.Add((item, "Duplicate in database"));
                        continue;
                    }

                    // Passed all checks — add to DB
                    var entity = new IndentDump_Service
                    {
                        Indent_No = item.Indent_No,
                        Indent_Date = item.Indent_Date,
                        Business_Unit = item.Business_Unit,
                        Vertical = item.Vertical,
                        Branch = item.Branch,
                        Indent_Status = item.Indent_Status,
                        End_Cust_Name = item.End_Cust_Name,
                        Complaint_Id = item.Complaint_Id,
                        Customer_Code = item.Customer_Code,
                        Customer_Name = item.Customer_Name,
                        Bill_Req_Date = item.Bill_Req_Date,
                        Created_By = item.Created_By,
                        Wipro_Commit_Date = item.Wipro_Commit_Date,
                        Material_No = item.Material_No,
                        Item_Description = item.Item_Description,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Final_Price = item.Final_Price,
                        SapSoNo = item.SapSoNo,
                        CreateSoQty = item.CreateSoQty,
                        Inv_Qty = item.Inv_Qty,
                        Inv_Value = item.Inv_Value,
                        WiproCatelog_No = item.WiproCatelog_No,
                        Batch_Code = item.Batch_Code,
                        Batch_Date = item.Batch_Date,
                        Main_Prodcode = item.Main_Prodcode,
                        User_Name = item.User_Name,
                        CreatedBy = item.CreatedBy,
                        CreatedDate = item.CreatedDate
                    };

                    _dbContext.IndentDump.Add(entity);
                }

                await _dbContext.SaveChangesAsync();

                // Log the upload
                var importLog = new FailedRecord_Log
                {
                    FileName = fileName,
                    TotalRecords = listOfData.Count,
                    ImportedRecords = listOfData.Count - result.FailedRecords.Count,
                    FailedRecords = result.FailedRecords.Count,
                    RecordType = recordType,
                    UploadedBy = uploadedBy,
                    UploadedAt = DateTime.Now
                };

                _dbContext.FailedRecord_Log.Add(importLog);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Result = new OperationResult
                {
                    Success = true,
                    Message = result.FailedRecords.Any()
                        ? "Import completed with some skipped records."
                        : "All records imported successfully."
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Result = new OperationResult
                {
                    Success = false,
                    Message = "Error during import: " + ex.Message
                };
            }

            return result;
        }

        //// ----------------- Indent Dump ------------------- ////


        public async Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync()
        {
            return await _dbContext.Vendor
                .Where(v => !v.Deleted)
                .Select(v => new DropdownOptionViewModel
                {
                    Label = v.Name,
                    Value = v.Vendor_Code
                })
                .Distinct()
                .ToListAsync();
        }

    }
}

