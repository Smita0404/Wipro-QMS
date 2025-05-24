using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;

namespace QMS.Core.Repositories.RMTCDetailsRepository
{
    public class RMTCDetailsRepository : SqlTableRepository, IRMTCDetailsRepository
    {
        private readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public RMTCDetailsRepository(QMSDbContext dbContext, ISystemLogService systemLogService)
            : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<RMTCDetailsViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var result = await _dbContext.RMTCDetails
                    .FromSqlRaw("EXEC sp_Get_RMTCDetails")
                    .ToListAsync();

                // Apply optional date filtering based on RMTCDate
                if (startDate.HasValue && endDate.HasValue)
                {
                    result = result
                        .Where(x => x.RMTCDate.HasValue &&
                                    x.RMTCDate.Value.Date >= startDate.Value.Date &&
                                    x.RMTCDate.Value.Date <= endDate.Value.Date)
                        .ToList();
                }

                return result.Select(data => new RMTCDetailsViewModel
                {
                    Id = data.Id,
                    Vendor = data.Vendor,
                    ProductCatRef = data.ProductCatRef,
                    ProductDescription = data.ProductDescription,
                    RMTCDate = data.RMTCDate,
                    CreatedDate = data.CreatedDate,
                    UpdatedDate = data.UpdatedDate,
                    CreatedBy = data.CreatedBy,
                    UpdatedBy = data.UpdatedBy,
                    Remarks = data.Remarks,
                    HousingBody = data.HousingBody,
                    WiresCables = data.WiresCables,
                    DiffuserLens = data.DiffuserLens,
                    PCB = data.PCB,
                    Connectors = data.Connectors,
                    PowderCoat = data.PowderCoat,
                    LEDLM80PhotoBiological = data.LEDLM80PhotoBiological,
                    LEDPurchaseProof = data.LEDPurchaseProof,
                    Driver = data.Driver,
                    Pretreatment = data.Pretreatment,
                    Hardware = data.Hardware,
                    OtherCriticalItems = data.OtherCriticalItems,
                    Filename = data.Filename
                }).ToList();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }


        public async Task<RMTCDetails?> GetByIdAsync(int id)
        {
            try
            {
                var result = await _dbContext.RMTCDetails
                    .FromSqlRaw("EXEC sp_Get_RMTCDetails_ByID @RMTCId", new SqlParameter("@RMTCId", id))
                    .ToListAsync();

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(RMTCDetails entity,bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Vendor", entity.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCatRef", entity.ProductCatRef ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@RMTCDate", entity.RMTCDate ?? (object)DBNull.Value),
                    new SqlParameter("@HousingBody", entity.HousingBody ?? (object)DBNull.Value),
                    new SqlParameter("@WiresCables", entity.WiresCables ?? (object)DBNull.Value),
                    new SqlParameter("@DiffuserLens", entity.DiffuserLens ?? (object)DBNull.Value),
                    new SqlParameter("@PCB", entity.PCB ?? (object)DBNull.Value),
                    new SqlParameter("@Connectors", entity.Connectors ?? (object)DBNull.Value),
                    new SqlParameter("@PowderCoat", entity.PowderCoat ?? (object)DBNull.Value),
                    new SqlParameter("@LEDLM80PhotoBiological", entity.LEDLM80PhotoBiological ?? (object)DBNull.Value),
                    new SqlParameter("@LEDPurchaseProof", entity.LEDPurchaseProof ?? (object)DBNull.Value),
                    new SqlParameter("@Driver", entity.Driver ?? (object)DBNull.Value),
                    new SqlParameter("@Pretreatment", entity.Pretreatment ?? (object)DBNull.Value),
                    new SqlParameter("@Hardware", entity.Hardware ?? (object)DBNull.Value),
                    new SqlParameter("@OtherCriticalItems", entity.OtherCriticalItems ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@Filename", entity.Filename ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", entity.CreatedBy ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Insert_RMTCDetails " +
                    "@Vendor, @ProductCatRef, @ProductDescription, @RMTCDate, @HousingBody, @WiresCables, " +
                    "@DiffuserLens, @PCB, @Connectors, @PowderCoat, @LEDLM80PhotoBiological, @LEDPurchaseProof, " +
                    "@Driver, @Pretreatment, @Hardware, @OtherCriticalItems, @Remarks, @Filename, @CreatedBy",
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

        public async Task<OperationResult> UpdateAsync(RMTCDetails entity, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@RMTCId", entity.Id),
                    new SqlParameter("@Vendor", entity.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@ProductCatRef", entity.ProductCatRef ?? (object)DBNull.Value),
                    new SqlParameter("@ProductDescription", entity.ProductDescription ?? (object)DBNull.Value),
                    new SqlParameter("@RMTCDate", entity.RMTCDate ?? (object)DBNull.Value),
                    new SqlParameter("@HousingBody", entity.HousingBody ?? (object)DBNull.Value),
                    new SqlParameter("@WiresCables", entity.WiresCables ?? (object)DBNull.Value),
                    new SqlParameter("@DiffuserLens", entity.DiffuserLens ?? (object)DBNull.Value),
                    new SqlParameter("@PCB", entity.PCB ?? (object)DBNull.Value),
                    new SqlParameter("@Connectors", entity.Connectors ?? (object)DBNull.Value),
                    new SqlParameter("@PowderCoat", entity.PowderCoat ?? (object)DBNull.Value),
                    new SqlParameter("@LEDLM80PhotoBiological", entity.LEDLM80PhotoBiological ?? (object)DBNull.Value),
                    new SqlParameter("@LEDPurchaseProof", entity.LEDPurchaseProof ?? (object)DBNull.Value),
                    new SqlParameter("@Driver", entity.Driver ?? (object)DBNull.Value),
                    new SqlParameter("@Pretreatment", entity.Pretreatment ?? (object)DBNull.Value),
                    new SqlParameter("@Hardware", entity.Hardware ?? (object)DBNull.Value),
                    new SqlParameter("@OtherCriticalItems", entity.OtherCriticalItems ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", entity.Remarks ?? (object)DBNull.Value),
                    new SqlParameter("@Filename", entity.Filename ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", entity.UpdatedBy ?? (object)DBNull.Value)
                };

                await _dbContext.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Update_RMTCDetails " +
                    "@RMTCId, @Vendor, @ProductCatRef, @ProductDescription, @RMTCDate, @HousingBody, @WiresCables, " +
                    "@DiffuserLens, @PCB, @Connectors, @PowderCoat, @LEDLM80PhotoBiological, @LEDPurchaseProof, " +
                    "@Driver, @Pretreatment, @Hardware, @OtherCriticalItems, @Remarks, @Filename, @UpdatedBy",
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
                return await base.DeleteAsync<RMTCDetails>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync()
        {
            try
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

    }
}
