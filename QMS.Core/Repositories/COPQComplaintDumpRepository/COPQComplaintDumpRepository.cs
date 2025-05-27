using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;


namespace QMS.Core.Repositories.COPQComplaintDumpRepository
{
    public class COPQComplaintDumpRepository : SqlTableRepository, ICOPQComplaintDumpRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public COPQComplaintDumpRepository(QMSDbContext dbContext, ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<COPQComplaintDumpViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
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

                return result.Select(x => new COPQComplaintDumpViewModel
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

        public async Task<OperationResult> CreateAsync(COPQComplaintDump record, bool returnCreatedRecord = false)
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

        public async Task<OperationResult> UpdateAsync(COPQComplaintDump record, bool returnUpdatedRecord = false)
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
                return await base.DeleteAsync<COPQComplaintDump>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<COPQComplaintDumpViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@Id", id) };

                var result = await _dbContext.COPQComplaintDump
                    .FromSqlRaw("EXEC sp_Get_COPQComplaintDump_ById @Id", parameters)
                    .ToListAsync();

                return result.Select(x => new COPQComplaintDumpViewModel
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




            public async Task<List<PODetailViewModel>> GetPOListAsync(DateTime? startDate = null, DateTime? endDate = null)
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

                    return result.Select(x => new PODetailViewModel
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

            public async Task<PODetailViewModel?> GetPOByIdAsync(int id)
            {
                try
                {
                    var param = new SqlParameter("@Id", id);
                    var result = await _dbContext.PODetails
                        .FromSqlRaw("EXEC sp_Get_POById @Id", param)
                        .ToListAsync();

                    return result.Select(x => new PODetailViewModel
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

            public async Task<OperationResult> CreatePOAsync(PODetail podetail, bool returnCreatedRecord = false)
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

            public async Task<OperationResult> UpdatePOAsync(PODetail podetail, bool returnUpdatedRecord = false)
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
                    return await base.DeleteAsync<PODetail>(id);
                }
                catch (Exception ex)
                {
                    _systemLogService.WriteLog($"DeletePOAsync: {ex.Message}");
                    throw;
                }
            }
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

