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

        public async Task<List<COPQComplaintDumpViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.COPQComplaintDump
                    .FromSqlRaw("EXEC sp_Get_COPQComplaintDump")
                    .ToListAsync();

                //if (startDate.HasValue && endDate.HasValue)
                //{
                //    result = result
                //        .Where(x => x.CCCNDate.HasValue &&
                //                    x.CCCNDate.Value.Date >= startDate.Value.Date &&
                //                    x.CCCNDate.Value.Date <= endDate.Value.Date)
                //        .ToList();
                //}

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
    }
}
