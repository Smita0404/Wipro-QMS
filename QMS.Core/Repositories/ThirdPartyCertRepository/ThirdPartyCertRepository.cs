using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.CertificateMasterRepository;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.ThirdPartyCertRepository
{
    public class ThirdPartyCertRepository : SqlTableRepository, IThirdPartyCertRepository
    {
        private new readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public ThirdPartyCertRepository(QMSDbContext dbContext,
                               ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext= dbContext;
            _systemLogService = systemLogService;
        }
      
        public async Task<List<ThirdPartyCertificateMasterViewModel>> GetCertificatesAsync()
        {
            return await _dbContext.ThirdPartyCertificateMasters
                .Where(c => c.Deleted == false)  // Filter for non-deleted records
                .Select(c => new ThirdPartyCertificateMasterViewModel
                {
                    CertificateID = c.Id,
                    CertificateName = c.CertificateName
                })
                .ToListAsync();
        }

        public async Task<bool> CheckDuplicate(string searchText, int Id)
        {
            try
            {
                bool existingflag = false;
                int? existingId = null;

                IQueryable<int> query = _dbContext.ThirdPartyCertificateMasters
                    .Where(x => x.Deleted == false && x.CertificateName.ToString() == searchText)
                    .Select(x => x.Id);

                // Add additional condition if Id is not 0
                if (Id != 0)
                {
                    query = _dbContext.ThirdPartyCertificateMasters
                        .Where(x => x.Deleted == false &&
                               x.CertificateName.ToString() == searchText
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
        public async Task<ThirdPartyCertificateMasterViewModel?> GetCertificateByIdAsync(int certificateId)
        {
            return await _dbContext.ThirdPartyCertificateMasters
                .Where(c => c.Id == certificateId)
                .Select(c => new ThirdPartyCertificateMasterViewModel
                {
                    CertificateID = c.Id,
                    CertificateName = c.CertificateName
                }).FirstOrDefaultAsync();
        }

        public async Task<OperationResult> CreateCertificateAsync(ThirdPartyCertificateMaster model, bool checkDuplicate)
        {
            var result = new OperationResult();

            if (checkDuplicate)
            {
                bool exists = await CheckDuplicate(model.CertificateName, model.Id);
                if (exists)
                {
                    result.Success = false;
                    result.Message = "Exist";
                    return result;
                }
            }

            _dbContext.ThirdPartyCertificateMasters.Add(model);
            await _dbContext.SaveChangesAsync();

            result.Success = true;
            result.Message = "Saved";
            result.ObjectId = model.Id;
            return result;
        }
        public async Task<OperationResult> DeleteAsync(int Id,string updatedby)
        {
            try
            {
               
                var certificate = await _dbContext.ThirdPartyCertificateMasters.FirstOrDefaultAsync(c => c.Id == Id);

                if (certificate != null)
                {
                    certificate.Deleted = true; 
                    certificate.UpdatedDate = DateTime.Now;
                    certificate.UpdatedBy = updatedby;

                    await _dbContext.SaveChangesAsync();

                    // Return success result
                    return new OperationResult
                    {
                        Success = true,
                        Message = "Certificate successfully deleted."
                    };
                }

                // If certificate not found
                return new OperationResult
                {
                    Success = false,
                    Message = "Certificate not found."
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                _systemLogService.WriteLog(ex.Message);

                // Return failure result
                return new OperationResult
                {
                    Success = false,
                    Message = "Error occurred while deleting certificate."
                };
            }
        }


        public async Task<OperationResult> UpdateCertificateAsync(ThirdPartyCertificateMasterViewModel model)
        {
            try
            {
                var certificate = await _dbContext.ThirdPartyCertificateMasters
                    .FirstOrDefaultAsync(c => c.Id == model.CertificateID);

                if (certificate == null)
                    return new OperationResult { Success = false, Message = "Certificate not found." };

                certificate.CertificateName = model.CertificateName;
                certificate.UpdatedBy = model.UpdatedBy;
                certificate.UpdatedDate = model.UpdatedDate;

                await _dbContext.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Certificate updated successfully." };
            }
            catch (Exception ex)
            {
                _systemLogService.WriteLog($"Error updating certificate: {ex.Message}");
                return new OperationResult { Success = false, Message = "An error occurred while updating the certificate." };
            }
        }


    }
}