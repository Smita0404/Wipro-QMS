using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;


namespace QMS.Core.Repositories.CSOTrackerRepository
{
    public class CSOTrackerRepository : SqlTableRepository, ICSOTrackerRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public CSOTrackerRepository(QMSDbContext dbContext, ISystemLogService systemLogService)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<CSOTrackerViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.CSOTracker.FromSqlRaw("EXEC sp_Get_CSO_Tracker").ToListAsync();

                return result.Select(data => new CSOTrackerViewModel
                {
                    CSOId = data.Id,
                    CSOLogDate = data.CSOLogDate,
                    CSONo = data.CSONo,
                    ClassAB = data.ClassAB,
                    ProductCatRef = data.ProductCatRef,
                    ProductDescription = data.ProductDescription,
                    SourceOfCSO = data.SourceOfCSO,
                    InternalExternal = data.InternalExternal,
                    PKDBatchCode = data.PKDBatchCode,
                    ProblemStatement = data.ProblemStatement,
                    SuppliedQty = data.SuppliedQty,
                    FailedQty = data.FailedQty,
                    RootCause = data.RootCause,
                    CorrectiveAction = data.CorrectiveAction,
                    PreventiveAction = data.PreventiveAction,
                    CSOsClosureDate = data.CSOsClosureDate,
                    Aging = data.Aging,
                    AttachmentCAPAReport = data.AttachmentCAPAReport
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<CSOTracker?> GetByIdAsync(int id)
        {
            try
            {
                var result = await _dbContext.CSOTracker
                    .FromSqlRaw("EXEC sp_Get_CSOTracker_ByID @CSOId", new SqlParameter("@CSOId", id))
                    .ToListAsync();

                return result.Select(data => new CSOTracker
                {
                   // CSOId = data.Id,
                    CSOLogDate = data.CSOLogDate,
                    CSONo = data.CSONo,
                    ClassAB = data.ClassAB,
                    ProductCatRef = data.ProductCatRef,
                    ProductDescription = data.ProductDescription,
                    SourceOfCSO = data.SourceOfCSO,
                    InternalExternal = data.InternalExternal,
                    PKDBatchCode = data.PKDBatchCode,
                    ProblemStatement = data.ProblemStatement,
                    SuppliedQty = data.SuppliedQty,
                    FailedQty = data.FailedQty,
                    RootCause = data.RootCause,
                    CorrectiveAction = data.CorrectiveAction,
                    PreventiveAction = data.PreventiveAction,
                    CSOsClosureDate = data.CSOsClosureDate,
                    Aging = data.Aging,
                    AttachmentCAPAReport = data.AttachmentCAPAReport
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(CSOTracker entity, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CSOLogDate", entity.CSOLogDate ?? (object)DBNull.Value),
                    new SqlParameter("@CSONo", entity.CSONo ?? (object)DBNull.Value),
                    new SqlParameter("@ClassAB", entity.ClassAB ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCatRef", entity.ProductCatRef ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@SourceOfCSO", entity.SourceOfCSO ?? (object)DBNull.Value),
                    new SqlParameter("@InternalExternal", entity.InternalExternal ?? (object)DBNull.Value),
                    new SqlParameter("@PKDBatchCode", entity.PKDBatchCode ?? (object)DBNull.Value),
                    new SqlParameter("@ProblemStatement", entity.ProblemStatement ?? (object)DBNull.Value),
                    new SqlParameter("@SuppliedQty", entity.SuppliedQty ?? (object)DBNull.Value),
                    new SqlParameter("@FailedQty", entity.FailedQty ?? (object)DBNull.Value),
                    new SqlParameter("@RootCause", entity.RootCause ?? (object)DBNull.Value),
                    new SqlParameter("@CorrectiveAction", entity.CorrectiveAction ?? (object)DBNull.Value),
                    new SqlParameter("@PreventiveAction", entity.PreventiveAction ?? (object)DBNull.Value),
                    new SqlParameter("@CSOsClosureDate", entity.CSOsClosureDate ?? (object)DBNull.Value),
                    new SqlParameter("@Aging", entity.Aging ?? (object)DBNull.Value),
                    new SqlParameter("@AttachmentCAPAReport", entity.AttachmentCAPAReport ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", entity.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync("EXEC sp_Insert_CSOTracker @CSOLogDate, @CSONo, @ClassAB, @ProductCatRef, @ProductDescription, @SourceOfCSO, @InternalExternal, @PKDBatchCode, @ProblemStatement, @SuppliedQty, @FailedQty, @RootCause, @CorrectiveAction, @PreventiveAction, @CSOsClosureDate, @Aging, @AttachmentCAPAReport, @IsDeleted", parameters);

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> UpdateAsync(CSOTracker entity, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@CSOId", entity.Id),
                    new SqlParameter("@CSOLogDate", entity.CSOLogDate ?? (object)DBNull.Value),
                    new SqlParameter("@CSONo", entity.CSONo ?? (object)DBNull.Value),
                    new SqlParameter("@ClassAB", entity.ClassAB ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCatRef", entity.ProductCatRef ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@SourceOfCSO", entity.SourceOfCSO ?? (object)DBNull.Value),
                    new SqlParameter("@InternalExternal", entity.InternalExternal ?? (object)DBNull.Value),
                    new SqlParameter("@PKDBatchCode", entity.PKDBatchCode ?? (object)DBNull.Value),
                    new SqlParameter("@ProblemStatement", entity.ProblemStatement ?? (object)DBNull.Value),
                    new SqlParameter("@SuppliedQty", entity.SuppliedQty ?? (object)DBNull.Value),
                    new SqlParameter("@FailedQty", entity.FailedQty ?? (object)DBNull.Value),
                    new SqlParameter("@RootCause", entity.RootCause ?? (object)DBNull.Value),
                    new SqlParameter("@CorrectiveAction", entity.CorrectiveAction ?? (object)DBNull.Value),
                    new SqlParameter("@PreventiveAction", entity.PreventiveAction ?? (object)DBNull.Value),
                    new SqlParameter("@CSOsClosureDate", entity.CSOsClosureDate ?? (object)DBNull.Value),
                    new SqlParameter("@Aging", entity.Aging ?? (object)DBNull.Value),
                    new SqlParameter("@AttachmentCAPAReport", entity.AttachmentCAPAReport ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync("EXEC sp_Update_CSOTracker @CSOId, @CSOLogDate, @CSONo, @ClassAB, @ProductCatRef, @ProductDescription, @SourceOfCSO, @InternalExternal, @PKDBatchCode, @ProblemStatement, @SuppliedQty, @FailedQty, @RootCause, @CorrectiveAction, @PreventiveAction, @CSOsClosureDate, @Aging, @AttachmentCAPAReport", parameters);

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
                return await base.DeleteAsync<CSOTracker>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<bool> CheckDuplicateAsync(string csoNo, int id)
        {
            try
            {
                var existing = await _dbContext.CSOTracker
                    .Where(x => x.CSONo == csoNo && !x.Deleted && x.Id != id)
                    .FirstOrDefaultAsync();

                return existing != null;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
