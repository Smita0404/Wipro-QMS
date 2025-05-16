using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Services.SystemLogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.ThirdPartyInspectionRepository
{
    public class ThirdPartyInspectionRepository : IThirdPartyInspectionRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public ThirdPartyInspectionRepository(QMSDbContext dbContext, ISystemLogService systemLogService)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<ThirdPartyInspectionViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.ThirdPartyInspections.FromSqlRaw("EXEC sp_Get_ThirdPartyInspections").ToListAsync();

                // Map results to ViewModel
                var viewModelList = result.Select(data => new ThirdPartyInspectionViewModel
                {
                    InspectionID = data.Id,
                    ProjectName = data.ProjectName,
                    InspName = data.InspName,
                    ProductCode = data.ProductCode,
                    ProdDesc = data.ProdDesc,
                    LOTQty = data.LOTQty,
                    ProjectValue = data.ProjectValue,
                    Location = data.Location,
                    Mode = data.Mode,
                    FirstAttempt = data.FirstAttempt,
                    Remark = data.Remark,
                    ActionPlan = data.ActionPlan,
                    MOMDate = data.MOMDate,
                    Attachment = data.Attachment,
                    InspectionDate = data.InspectionDate
                    //  CreatedBy = data.CreatedBy,
                    // CreatedDate = data.CreatedDate,
                    // UpdatedBy = data.UpdatedBy,
                    //UpdatedDate = data.UpdatedDate,
                }).ToList();

                if (startDate.HasValue && endDate.HasValue)
                {
                    viewModelList = viewModelList
                        .Where(x => x.InspectionDate.HasValue &&
                                    x.InspectionDate.Value.Date >= startDate.Value.Date &&
                                    x.InspectionDate.Value.Date <= endDate.Value.Date)
                        .ToList();
                }

                return viewModelList;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
        public async Task UpdateAttachmentPath(ThirdPartyInspection attachment)
        {
            var existing = await _dbContext.ThirdPartyInspections.FindAsync(attachment.Id);
            if (existing == null) return;

            existing.Attachment = attachment.Attachment;
            existing.UpdatedDate = DateTime.Now;
            existing.UpdatedBy = attachment.UpdatedBy;

            await _dbContext.SaveChangesAsync();
        }
     
        public async Task UpdateAttachmentPathAsync(ThirdPartyInspection attachment)
        {
           
            var inspection = new ThirdPartyInspection
            {
                Id = attachment.Id,
                Attachment = string.IsNullOrWhiteSpace(attachment.Attachment) ? null : attachment.Attachment,
                UpdatedDate = DateTime.Now,
                UpdatedBy=attachment.UpdatedBy
                
            };

            _dbContext.ThirdPartyInspections.Attach(inspection);
            _dbContext.Entry(inspection).Property(x => x.Attachment).IsModified = true;
            _dbContext.Entry(inspection).Property(x => x.UpdatedDate).IsModified = true;

            await _dbContext.SaveChangesAsync();
        }


        public async Task<OperationResult> CreateAsync(ThirdPartyInspection newRecord, bool returnCreatedRecord = false)
        {
            var operationResult = new OperationResult();

            try
            {
                var parameters = new[]
                {
            new SqlParameter("@InspectionDate", newRecord.InspectionDate ?? (object)DBNull.Value),
            new SqlParameter("@ProjectName", newRecord.ProjectName ?? (object)DBNull.Value),
            new SqlParameter("@InspName", newRecord.InspName ?? (object)DBNull.Value),
            new SqlParameter("@ProductCode", newRecord.ProductCode ?? (object)DBNull.Value),
            new SqlParameter("@ProdDesc", newRecord.ProdDesc ?? (object)DBNull.Value),
            new SqlParameter("@LOTQty", newRecord.LOTQty ?? (object)DBNull.Value),
            new SqlParameter("@ProjectValue", newRecord.ProjectValue ?? (object)DBNull.Value),
            new SqlParameter("@Location", newRecord.Location ?? (object)DBNull.Value),
            new SqlParameter("@Mode", newRecord.Mode ?? (object)DBNull.Value),
            new SqlParameter("@FirstAttempt", newRecord.FirstAttempt ?? (object)DBNull.Value),
            new SqlParameter("@Remark", newRecord.Remark ?? (object)DBNull.Value),
            new SqlParameter("@ActionPlan", newRecord.ActionPlan ?? (object)DBNull.Value),
            new SqlParameter("@MOMDate", newRecord.MOMDate ?? (object)DBNull.Value),
            new SqlParameter("@Attachment", newRecord.Attachment ?? (object)DBNull.Value),
            new SqlParameter("@CreatedBy", newRecord.CreatedBy ?? (object)DBNull.Value),
            new SqlParameter("@CreatedDate", newRecord.CreatedDate ?? (object)DBNull.Value)
        };

                // Execute and get the inserted ID
                var result = await _dbContext
                    .InspectionResults
                    .FromSqlRaw("EXEC sp_Insert_ThirdPartyInspection @InspectionDate,@ProjectName,@InspName,@ProductCode,@ProdDesc,@LOTQty,@ProjectValue,@Location,@Mode,@FirstAttempt,@Remark,@ActionPlan,@MOMDate,@Attachment,@CreatedBy,@CreatedDate", parameters)
                    .ToListAsync();

                var insertedId = result.FirstOrDefault()?.InspectionID ?? 0;

                operationResult.Success = insertedId > 0;
                operationResult.ObjectId = insertedId;
                return operationResult;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                operationResult.Success = false;
                operationResult.Message = "Error: " + ex.Message;
                return operationResult;
            }
        }

        public async Task<OperationResult> UpdateAsync(ThirdPartyInspection updatedRecord, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@InspectionID", updatedRecord.Id),
                    new SqlParameter("@InspectionDate", updatedRecord.InspectionDate ?? (object)DBNull.Value),
                    new SqlParameter("@ProjectName", updatedRecord.ProjectName ?? (object)DBNull.Value),
                    new SqlParameter("@InspName", updatedRecord.InspName ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCode", updatedRecord.ProductCode ?? (object)DBNull.Value),
                    new SqlParameter("@ProdDesc", updatedRecord.ProdDesc ?? (object)DBNull.Value),
                    new SqlParameter("@LOTQty", updatedRecord.LOTQty),
                    new SqlParameter("@ProjectValue", updatedRecord.ProjectValue ?? (object)DBNull.Value),
                    new SqlParameter("@Location", updatedRecord.Location ?? (object)DBNull.Value),
                    new SqlParameter("@Mode", updatedRecord.Mode ?? (object)DBNull.Value),
                    new SqlParameter("@FirstAttempt", updatedRecord.FirstAttempt ?? (object)DBNull.Value),
                    new SqlParameter("@Remark", updatedRecord.Remark ?? (object)DBNull.Value),
                    new SqlParameter("@ActionPlan", updatedRecord.ActionPlan ?? (object)DBNull.Value),
                    new SqlParameter("@MOMDate", updatedRecord.MOMDate ?? (object)DBNull.Value),
                    new SqlParameter("@Attachment", updatedRecord.Attachment ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", updatedRecord.UpdatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedDate", updatedRecord.UpdatedDate ?? (object)DBNull.Value)
                   // new SqlParameter("@IsDeleted", updatedRecord.Deleted),
                };

                var sql = @"EXEC sp_Update_ThirdPartyInspection @InspectionID,@InspectionDate,@ProjectName,@InspName,@ProductCode,@ProdDesc,@LOTQty,@ProjectValue,@Location,@Mode,@FirstAttempt,@Remark,@ActionPlan,@MOMDate,@Attachment,@UpdatedBy,@UpdatedDate";

                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                if (returnUpdatedRecord)
                {
                    return new OperationResult
                    {
                        Success = true,
                    };
                }

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> DeleteAsync(int inspectionId,string UpdatedBy)
        {
            try
            {
                var entity = await _dbContext.ThirdPartyInspections.FindAsync(inspectionId);
                if (entity != null)
                {
                    entity.Deleted = true;
                    entity.UpdatedDate = DateTime.Now;
                    entity.UpdatedBy = UpdatedBy;

                    _dbContext.ThirdPartyInspections.Update(entity);
                    await _dbContext.SaveChangesAsync();

                    
                }

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return new OperationResult { Success = false, Message = ex.Message };
            }
        }
      
        public async Task<ThirdPartyInspection> GetByIdAsync(int inspectionId)
        {
            try
            {
                var parameters = new[] { new SqlParameter("@InspectionID", inspectionId) };

                var sql = @"EXEC sp_Get_ThirdPartyInspection_By_Id @InspectionID";

                var result = await _dbContext.ThirdPartyInspections.FromSqlRaw(sql, parameters).ToListAsync();

                var viewModelList = result.Select(data => new ThirdPartyInspection
                {
                    Id = data.Id,
                    ProjectName = data.ProjectName,
                    InspName = data.InspName,
                    ProductCode = data.ProductCode,
                    ProdDesc = data.ProdDesc,
                    LOTQty = data.LOTQty,
                    ProjectValue = data.ProjectValue,
                    Location = data.Location,
                    Mode = data.Mode,
                    FirstAttempt = data.FirstAttempt,
                    Remark = data.Remark,
                    ActionPlan = data.ActionPlan,
                    MOMDate = data.MOMDate,
                    Attachment = data.Attachment,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedDate = data.UpdatedDate,
                }).ToList();

                return viewModelList.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
        public async Task<OperationResult> RemoveAttachmentAsync(int inspectionId, string filePath, string updatedBy)
        {
            try
            {
                var record = await _dbContext.ThirdPartyInspections
                    .Where(x => x.Id == inspectionId && !x.Deleted)
                    .FirstOrDefaultAsync();

                if (record == null || string.IsNullOrWhiteSpace(record.Attachment))
                {
                    return new OperationResult { Success = false, Message = "Record not found or no attachment exists." };
                }

                var attachments = record.Attachment.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                if (!attachments.Contains(filePath))
                {
                    return new OperationResult { Success = false, Message = "Attachment not found in record." };
                }

                attachments.Remove(filePath);
                record.Attachment = attachments.Any() ? string.Join(";", attachments) : null;
                record.UpdatedDate = DateTime.Now;
                record.UpdatedBy = updatedBy;

                // Delete physical file (adjust this path as needed)
                var fullPath = Path.Combine("wwwroot", filePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                await _dbContext.SaveChangesAsync();

                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                return new OperationResult { Success = false, Message = "Error: " + ex.Message };
            }
        }

        public async Task<bool> CheckDuplicate(string projectName, int inspectionId)
        {
            try
            {
                bool existingFlag = false;
                int? existingId = null;

                IQueryable<int> query = _dbContext.ThirdPartyInspections
                    .Where(x => x.Deleted == false && x.ProjectName == projectName)
                    .Select(x => x.Id);

                              if (inspectionId != 0)
                {
                    query = _dbContext.ThirdPartyInspections
                        .Where(x => x.Deleted == false && x.ProjectName == projectName && x.Id != inspectionId)
                        .Select(x => x.Id);
                }

                existingId = await query.FirstOrDefaultAsync();

                if (existingId != null && existingId > 0)
                {
                    existingFlag = true;
                }

                return existingFlag;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
