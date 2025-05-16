using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;

namespace QMS.Core.Repositories.PDITrackerRepository
{
    public class PDITrackerRepository : SqlTableRepository, IPDITrackerRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public PDITrackerRepository(QMSDbContext dbContext, ISystemLogService systemLogService)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<PDITrackerViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.PDITracker
                    .FromSqlRaw("EXEC sp_Get_PDI_Tracker")
                    .ToListAsync();

                return result.Select(data => new PDITrackerViewModel
                {
                    Id = data.Id,
                    DispatchDate = data.DispatchDate.HasValue ? DateOnly.FromDateTime(data.DispatchDate.Value) : (DateOnly?)null,
                    PC = data.PC,
                    ProductCode = data.ProductCode,
                    ProductDescription = data.ProductDescription,
                    BatchCodeVendor = data.BatchCodeVendor,
                    PONo = data.PONo,
                    PDIDate = data.PDIDate.HasValue ? DateOnly.FromDateTime(data.PDIDate.Value) : (DateOnly?)null,
                    PDIRefNo = data.PDIRefNo,
                    OfferedQty = data.OfferedQty,
                    ClearedQty = data.ClearedQty,
                    BISCompliance = data.BISCompliance,
                    InspectedBy = data.InspectedBy,
                    Remark = data.Remark,
                    IsDelete = data.Deleted
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }


        public async Task<PDITracker?> GetByIdAsync(int id)
        {
            try
            {
                var result = await _dbContext.PDITracker
                    .FromSqlRaw("EXEC sp_Get_PDITracker_ByID @PDIId", new SqlParameter("@PDIId", id))
                    .ToListAsync();

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(PDITracker entity, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@DispatchDate", entity.DispatchDate ?? (object)DBNull.Value),
                    new SqlParameter("@PC", entity.PC ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCode", entity.ProductCode ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@BatchCodeVendor", entity.BatchCodeVendor ?? (object)DBNull.Value),
                    new SqlParameter("@PONo", entity.PONo ?? (object)DBNull.Value),
                    new SqlParameter("@PDIDate", entity.PDIDate ?? (object)DBNull.Value),
                    new SqlParameter("@PDIRefNo", entity.PDIRefNo ?? (object)DBNull.Value),
                    new SqlParameter("@OfferedQty", entity.OfferedQty ?? (object)DBNull.Value),
                    new SqlParameter("@ClearedQty", entity.ClearedQty ?? (object)DBNull.Value),
                    new SqlParameter("@BISCompliance", entity.BISCompliance ?? (object)DBNull.Value),
                    new SqlParameter("@InspectedBy", entity.InspectedBy ?? (object)DBNull.Value),
                    new SqlParameter("@Remark", entity.Remark ?? (object)DBNull.Value),
                   new SqlParameter("@IsDeleted", entity.Deleted)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Insert_PDITracker @DispatchDate, @PC, @ProductCode, @ProductDescription, @BatchCodeVendor, @PONo, @PDIDate, @PDIRefNo, @OfferedQty, @ClearedQty, @BISCompliance, @InspectedBy, @Remark, @IsDeleted",
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

        public async Task<OperationResult> UpdateAsync(PDITracker entity, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PDIId", entity.Id),
                    new SqlParameter("@DispatchDate", entity.DispatchDate ?? (object)DBNull.Value),
                    new SqlParameter("@PC", entity.PC ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCode", entity.ProductCode ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@BatchCodeVendor", entity.BatchCodeVendor ?? (object)DBNull.Value),
                    new SqlParameter("@PONo", entity.PONo ?? (object)DBNull.Value),
                    new SqlParameter("@PDIDate", entity.PDIDate ?? (object)DBNull.Value),
                    new SqlParameter("@PDIRefNo", entity.PDIRefNo ?? (object)DBNull.Value),
                    new SqlParameter("@OfferedQty", entity.OfferedQty ?? (object)DBNull.Value),
                    new SqlParameter("@ClearedQty", entity.ClearedQty ?? (object)DBNull.Value),
                    new SqlParameter("@BISCompliance", entity.BISCompliance ?? (object)DBNull.Value),
                    new SqlParameter("@InspectedBy", entity.InspectedBy ?? (object)DBNull.Value),
                    new SqlParameter("@Remark", entity.Remark ?? (object)DBNull.Value),
                   
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Update_PDITracker @PDIId, @DispatchDate, @PC, @ProductCode, @ProductDescription, @BatchCodeVendor, @PONo, @PDIDate, @PDIRefNo, @OfferedQty, @ClearedQty, @BISCompliance, @InspectedBy, @Remark",
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

        public async Task<List<ProductCodeDetailViewModel>> GetCodeSearchAsync(string search = "")
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@OldPartNo", search),
                };

                var sql = @"EXEC sp_GetProductCode_Detail_ByCode @OldPartNo";

                var result = await _dbContext.ProductCode.FromSqlRaw(sql, parameters).ToListAsync();

                var viewModelList = result.Select(data => new ProductCodeDetailViewModel
                {
                    PCDetails_Id = data.PCDetails_Id,
                    OldPart_No = data.OldPart_No,
                    Description = data.Description
                }).ToList();

                return viewModelList; // Assuming you want a single view model based on the ID
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
                return await base.DeleteAsync<PDITracker>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
        public async Task<List<ProductCodeDetailViewModel>> GetCodeSelect2OptionsAsync(string search = "")
        {
            var parameters = new[] {
        new SqlParameter("@OldPartNo", search),
    };

            var sql = @"EXEC sp_GetProductCode_Detail_ByCode @OldPartNo";
            var result = await _dbContext.ProductCode.FromSqlRaw(sql, parameters).ToListAsync();

            return result.Select(data => new ProductCodeDetailViewModel
            {
                PCDetails_Id = data.PCDetails_Id,
                OldPart_No = data.OldPart_No,
                Description = data.Description
            }).ToList();
        }

       

        //public async Task<bool> CheckDuplicateAsync(string pdiRefNo, int id)
        //{
        //    try
        //    {
        //        var existing = await _dbContext.PDITracker
        //            .Where(x => x.PDIRefNo == pdiRefNo && !x.Deleted && x.Id != id)
        //            .FirstOrDefaultAsync();

        //        return existing != null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _systemLogService.WriteLog(ex.Message);
        //        throw;
        //    }
        //}
    }
}
