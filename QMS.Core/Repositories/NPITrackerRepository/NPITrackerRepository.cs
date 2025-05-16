using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.BisProjectTracRepository;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.NPITrackerRepository
{
    public class NPITrackerRepository : SqlTableRepository, INPITrackerRepository
    {
        private new readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public NPITrackerRepository(QMSDbContext dbContext,
                               ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;
        }

        public async Task<List<NPI_TarcViewModel>> GetListAsync()
        {
            try
            {
                var result = await _dbContext.NPITracker.FromSqlRaw("EXEC sp_Get_NPI_Tracker").ToListAsync();

                // Map results to ViewModel
                var viewModelList = result.Select(data => new NPI_TarcViewModel
                {
                    Id = data.Id,
                    PC = data.PC,
                    Vendor = data.Vendor,
                    Prod_Category = data.Prod_Category,
                    Product_Code = data.Product_Code,
                    Product_Des = data.Product_Des,
                    Wattage = data.Wattage,
                    NPI_Category = data.NPI_Category,
                    Offered_Date = data.Offered_Date,
                    Releasded_Day = data.Releasded_Day,
                    Released_Date = data.Released_Date,
                    Validation_Rep_No = data.Validation_Rep_No,
                    Customer_Comp = data.Customer_Comp,
                    Remark = data.Remark,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedDate = data.UpdatedDate
                }).ToList();

                return viewModelList;

            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<OperationResult> CreateAsync(NPITracker newRecord, bool returnCreatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@PC", newRecord.PC ?? (object)DBNull.Value),
                    new SqlParameter("@Vendor", newRecord.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@Prod_Category", newRecord.Prod_Category ?? (object)DBNull.Value),
                    new SqlParameter("@Product_Code", newRecord.Product_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Product_Des", newRecord.Product_Des ?? (object)DBNull.Value),
                    new SqlParameter("@Wattage", newRecord.Wattage ?? (object)DBNull.Value),
                    new SqlParameter("@NPI_Category", newRecord.NPI_Category ?? (object)DBNull.Value),
                    new SqlParameter("@Offered_Date", newRecord.Offered_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Released_Date", newRecord.Released_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Releasded_Day", newRecord.Releasded_Day ?? (object)DBNull.Value),
                    new SqlParameter("@Validation_Rep_No", newRecord.Validation_Rep_No ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Comp", newRecord.Customer_Comp ?? (object)DBNull.Value),
                    new SqlParameter("@Remark", newRecord.Remark ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", newRecord.CreatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@IsDeleted", newRecord.Deleted),
                };

                var sql = @"EXEC sp_Insert_NPI_Tracker @PC,@Vendor,@Prod_Category,@Product_Code,@Product_Des,@Wattage,@NPI_Category,@Offered_Date,@Released_Date,@Releasded_Day,
                        @Validation_Rep_No,@Customer_Comp,@Remark,@CreatedBy,@IsDeleted";

                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

                if (returnCreatedRecord)
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

        public async Task<OperationResult> UpdateAsync(NPITracker updatedRecord, bool returnUpdatedRecord = false)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@NPI_TracID", updatedRecord.Id),
                    new SqlParameter("@PC", updatedRecord.PC ?? (object)DBNull.Value),
                    new SqlParameter("@Vendor", updatedRecord.Vendor ?? (object)DBNull.Value),
                    new SqlParameter("@Prod_Category", updatedRecord.Prod_Category ?? (object)DBNull.Value),
                    new SqlParameter("@Product_Code", updatedRecord.Product_Code ?? (object)DBNull.Value),
                    new SqlParameter("@Product_Des", updatedRecord.Product_Des ?? (object)DBNull.Value),
                    new SqlParameter("@Wattage", updatedRecord.Wattage ?? (object)DBNull.Value),
                    new SqlParameter("@NPI_Category", updatedRecord.NPI_Category ?? (object)DBNull.Value),
                    new SqlParameter("@Offered_Date", updatedRecord.Offered_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Released_Date", updatedRecord.Released_Date ?? (object)DBNull.Value),
                    new SqlParameter("@Releasded_Day", updatedRecord.Releasded_Day ?? (object)DBNull.Value),
                    new SqlParameter("@Validation_Rep_No", updatedRecord.Validation_Rep_No ?? (object)DBNull.Value),
                    new SqlParameter("@Customer_Comp", updatedRecord.Customer_Comp ?? (object)DBNull.Value),
                    new SqlParameter("@Remark", updatedRecord.Remark ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", updatedRecord.UpdatedBy ?? (object)DBNull.Value),
                };

                var sql = @"EXEC sp_Update_NPI_Tracker @NPI_TracID,@PC,@Vendor,@Prod_Category,@Product_Code,@Product_Des,@Wattage,@NPI_Category,@Offered_Date,@Released_Date,@Releasded_Day,
                        @Validation_Rep_No,@Customer_Comp,@Remark,@UpdatedBy";

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

        public async Task<OperationResult> DeleteAsync(int id)
        {
            try
            {
                return await base.DeleteAsync<BisProject_Tracker>(id);
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<NPI_TarcViewModel?> GetByIdAsync(int npi_Id)
        {
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@NPI_TracID", npi_Id),
                };

                var sql = @"EXEC sp_Get_NPITrack_ByID @NPI_TracID";

                var result = await _dbContext.NPITracker.FromSqlRaw(sql, parameters).ToListAsync();

                var viewModelList = result.Select(data => new NPI_TarcViewModel
                {
                    Id = data.Id,
                    PC = data.PC,
                    Vendor = data.Vendor,
                    Prod_Category = data.Prod_Category,
                    Product_Code = data.Product_Code,
                    Product_Des = data.Product_Des,
                    Wattage = data.Wattage,
                    NPI_Category = data.NPI_Category,
                    Offered_Date = data.Offered_Date,
                    Releasded_Day = data.Releasded_Day,
                    Released_Date = data.Released_Date,
                    Validation_Rep_No = data.Validation_Rep_No,
                    Customer_Comp = data.Customer_Comp,
                    Remark = data.Remark,
                    CreatedBy = data.CreatedBy,
                    CreatedDate = data.CreatedDate,
                    UpdatedBy = data.UpdatedBy,
                    UpdatedDate = data.UpdatedDate

                }).ToList();

                return viewModelList.FirstOrDefault();

            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }

        public async Task<bool> CheckDuplicate(string searchText, int Id)
        {
            try
            {
                bool existingflag = false;
                int? existingId = null;

                IQueryable<int> query = _dbContext.NPITracker
                    .Where(x => x.Deleted == false && x.Product_Code.ToString() == searchText)
                    .Select(x => x.Id);

                // Add additional condition if Id is not 0
                if (Id != 0)
                {
                    query = _dbContext.NPITracker
                        .Where(x => x.Deleted == false &&
                               x.Product_Code.ToString() == searchText
                               && x.Id != Id)
                        .Select(x => x.Id);
                }


                existingId = await query.FirstOrDefaultAsync();

                if (existingId != null && existingId > 0)
                {
                    existingflag = true;
                }

                return existingflag;
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog(ex.Message);
                throw;
            }
        }
    }
}
