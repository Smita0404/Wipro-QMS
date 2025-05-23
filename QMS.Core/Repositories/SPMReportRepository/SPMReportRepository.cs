using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;

namespace QMS.Core.Repositories.SPMReportRepository
{
    public class SPMReportRepository : SqlTableRepository, ISPMReportRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _log;

        public SPMReportRepository(QMSDbContext dbContext, ISystemLogService log)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _log = log;
        }

        public async Task<List<SPMReportViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.SPMReports
                    .FromSqlRaw("EXEC sp_Get_SPMReport")
                    .ToListAsync();

                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result
                        .Where(x => x.CreatedDate.HasValue &&
                                    x.CreatedDate.Value.Date >= startDate.Value.Date &&
                                    x.CreatedDate.Value.Date <= endDate.Value.Date)
                        .ToList();
                }

                return result.Select(x => new SPMReportViewModel
                {
                    Id = x.Id,
                    VendorDetail = x.VendorDetail,
                    FY = x.FY,
                    SPMQuarter = x.SPMQuarter,
                    FinalStarRating = x.FinalStarRating,
                    Top2Parameter = x.Top2Parameter,
                    Lowest2Parameter = x.Lowest2Parameter,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,
                    Remarks = x.Remarks,
                    Deleted = x.Deleted
                }).ToList();
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<SPMReport?> GetByIdAsync(int id)
        {
            try
            {
                var result = await _dbContext.SPMReports
                    .FromSqlRaw("EXEC sp_Get_SPMReport_ById @Id", new SqlParameter("@Id", id))
                    .ToListAsync();

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(SPMReport entity)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@VendorDetail", entity.VendorDetail ?? (object)DBNull.Value),
            new SqlParameter("@FY", entity.FY ?? (object)DBNull.Value),
            new SqlParameter("@SPMQuarter", entity.SPMQuarter ?? (object)DBNull.Value),
            new SqlParameter("@FinalStarRating", (object?)entity.FinalStarRating ?? DBNull.Value),
            new SqlParameter("@Top2Parameter", entity.Top2Parameter ?? (object)DBNull.Value),
            new SqlParameter("@Lowest2Parameter", entity.Lowest2Parameter ?? (object)DBNull.Value),
            new SqlParameter("@CreatedBy", entity.CreatedBy ?? (object)DBNull.Value),
            new SqlParameter("@CreatedDate", entity.CreatedDate ?? (object)DBNull.Value),
            new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
            new SqlParameter("@IsDeleted", entity.Deleted)
        };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    @"EXEC sp_Insert_SPMReport 
              @VendorDetail, @FY, @SPMQuarter, @FinalStarRating, 
              @Top2Parameter, @Lowest2Parameter, @CreatedBy, 
              @CreatedDate, @Remarks, @IsDeleted", parameters);

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> UpdateAsync(SPMReport entity)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@Id", entity.Id),
            new SqlParameter("@VendorDetail", entity.VendorDetail ?? (object)DBNull.Value),
            new SqlParameter("@FY", entity.FY ?? (object)DBNull.Value),
            new SqlParameter("@SPMQuarter", entity.SPMQuarter ?? (object)DBNull.Value),
            new SqlParameter("@FinalStarRating", entity.FinalStarRating.HasValue ? entity.FinalStarRating.Value : (object)DBNull.Value),
            new SqlParameter("@Top2Parameter", entity.Top2Parameter ?? (object)DBNull.Value),
            new SqlParameter("@Lowest2Parameter", entity.Lowest2Parameter ?? (object)DBNull.Value),
            new SqlParameter("@UpdatedBy", entity.UpdatedBy ?? (object)DBNull.Value),
            new SqlParameter("@UpdatedDate", entity.UpdatedDate ?? (object)DBNull.Value),
            new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value)
        };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Update_SPMReport @Id, @VendorDetail, @FY, @SPMQuarter, @FinalStarRating, @Top2Parameter, @Lowest2Parameter, @UpdatedBy, @UpdatedDate, @Remarks",
                    parameters
                );

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _log.WriteLog($"[SPMReportRepository.UpdateAsync] {ex.Message}");
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
                return await base.DeleteAsync<SPMReport>(id);
            }
            catch (Exception ex)
            {
                _log.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
