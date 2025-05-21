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

namespace QMS.Core.Repositories.ImprTrackerRepository
{
    public class ImprTrackerRepository : SqlTableRepository, IImprTrackerRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public ImprTrackerRepository(QMSDbContext dbContext, ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<ImprovementTrackerViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.ImprovementTracker
                    .FromSqlRaw("EXEC sp_Get_ImprovementTracker")
                    .ToListAsync();

                return result.Select(x => new ImprovementTrackerViewModel
                {
                    Id = x.Id,
                    FuncArea = x.FuncArea,
                    Issue = x.Issue,
                    Problem = x.Problem,
                    CorrectiveAction = x.CorrectiveAction,
                    Responsible = x.Responsible,
                    StartDate = x.StartDate,
                    ImprAchieved = x.ImprAchieved,
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

        public async Task<OperationResult> CreateAsync(ImprovementTracker record, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@FuncArea", record.FuncArea ?? (object)DBNull.Value),
            new SqlParameter("@Issue", record.Issue ?? (object)DBNull.Value),
            new SqlParameter("@Problem", record.Problem ?? (object)DBNull.Value),
            new SqlParameter("@CorrectiveAction", record.CorrectiveAction ?? (object)DBNull.Value),
            new SqlParameter("@Responsible", record.Responsible ?? (object)DBNull.Value),
            new SqlParameter("@StartDate", record.StartDate ?? (object)DBNull.Value),
            new SqlParameter("@ImprAchieved", record.ImprAchieved ?? (object)DBNull.Value),
            new SqlParameter("@CreatedDate", DateTime.Now), // assuming current timestamp
            new SqlParameter("@CreatedBy", record.CreatedBy ?? (object)DBNull.Value),
            new SqlParameter("@IsDeleted", record.Deleted)
        };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Insert_ImprovementTracker @FuncArea, @Issue, @Problem, @CorrectiveAction, @Responsible, @StartDate, @ImprAchieved, @CreatedDate, @CreatedBy, @IsDeleted",
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

        public async Task<OperationResult> UpdateAsync(ImprovementTracker record, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Id", record.Id),
                
                    new SqlParameter("@FuncArea", record.FuncArea ?? (object)DBNull.Value),
                    new SqlParameter("@Issue", record.Issue ?? (object)DBNull.Value),
                    new SqlParameter("@Problem", record.Problem ?? (object)DBNull.Value),
                    new SqlParameter("@CorrectiveAction", record.CorrectiveAction ?? (object)DBNull.Value),
                    new SqlParameter("@Responsible", record.Responsible ?? (object)DBNull.Value),
                     new SqlParameter("@StartDate", record.StartDate ?? (object)DBNull.Value),
                    new SqlParameter("@ImprAchieved", record.ImprAchieved ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", record.UpdatedBy ?? (object)DBNull.Value)
                   
                };

                await _dbContext.Database.ExecuteSqlRawAsync("EXEC sp_Update_ImprovementTracker @Id, @FuncArea, @Issue, @Problem, @CorrectiveAction, @Responsible, @StartDate, @ImprAchieved, @UpdatedBy", parameters);

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
                return await base.DeleteAsync<ImprovementTracker>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<ImprovementTrackerViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@Id", id) };

                var result = await _dbContext.ImprovementTracker
                    .FromSqlRaw("EXEC sp_Get_ImprovementTracker_ById @Id", parameters)
                    .ToListAsync();

                return result.Select(x => new ImprovementTrackerViewModel
                {
                    Id = x.Id,
                   
                    FuncArea = x.FuncArea,
                    Issue = x.Issue,
                    Problem = x.Problem,
                    CorrectiveAction = x.CorrectiveAction,
                    Responsible = x.Responsible,
                    StartDate = x.StartDate,
                    ImprAchieved = x.ImprAchieved,
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
