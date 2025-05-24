using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.KaizenTrackerRepository
{
    public class KaizenTrackerRepository : SqlTableRepository, IKaizenTrackerRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public KaizenTrackerRepository(QMSDbContext dbContext, ISystemLogService systemLogService)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<KaizenTrackerViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.KaizenTracker.FromSqlRaw("EXEC sp_Get_KaizenTracker").ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result
                        .Where(x => x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Date >= startDate.Value.Date &&
                                    x.CreatedDate.Value.Date <= endDate.Value.Date)
                        .ToList();
                }

                return result.Select(data => new KaizenTrackerViewModel
                {
                    Id = data.Id,
                    Vendor = data.Vendor,
                    KaizenTheme = data.KaizenTheme,
                    KMonth = data.KMonth,
                    Team = data.Team,
                    KaizenFile = data.KaizenFile,
                    Remarks = data.Remarks,
                    CreatedDate = data.CreatedDate,
                    UpdatedDate = data.UpdatedDate,
                    CreatedBy = data.CreatedBy,
                    UpdatedBy = data.UpdatedBy,
                    IsDeleted = data.Deleted
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<KaizenTracker?> GetByIdAsync(int id)
        {
            try
            {
                var result = await _dbContext.KaizenTracker
                    .FromSqlRaw("EXEC sp_Get_KaizenTracker_ByID @Id", new SqlParameter("@Id", id))
                    .ToListAsync();

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(KaizenTracker entity, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Vendor", entity.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@KaizenTheme", entity.KaizenTheme ?? (object)DBNull.Value),
                    new SqlParameter("@KMonth", entity.KMonth ?? (object)DBNull.Value),
                    new SqlParameter("@Team", entity.Team ?? (object)DBNull.Value),
                    new SqlParameter("@KaizenFile", entity.KaizenFile ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", entity.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", entity.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Insert_KaizenTracker @Vendor, @KaizenTheme, @KMonth, @Team, @KaizenFile,  @CreatedBy,@Remarks, @IsDeleted",
                    parameters);

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> UpdateAsync(KaizenTracker entity, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", entity.Id),
                    new SqlParameter("@Vendor", entity.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@KaizenTheme", entity.KaizenTheme ?? (object)DBNull.Value),
                    new SqlParameter("@KMonth", entity.KMonth ?? (object)DBNull.Value),
                    new SqlParameter("@Team", entity.Team ?? (object)DBNull.Value),
                    new SqlParameter("@KaizenFile", entity.KaizenFile ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", entity.UpdatedBy ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Update_KaizenTracker @Id, @Vendor, @KaizenTheme, @KMonth, @Team, @KaizenFile, @UpdatedBy, @Remarks",
    parameters);

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
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
        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                return await base.DeleteAsync<KaizenTracker>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
