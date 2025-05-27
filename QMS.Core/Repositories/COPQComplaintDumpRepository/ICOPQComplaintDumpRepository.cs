using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.COPQComplaintDumpRepository
{
    public interface ICOPQComplaintDumpRepository
    {
        Task<List<COPQComplaintDumpViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<OperationResult> CreateAsync(COPQComplaintDump complaint, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(COPQComplaintDump complaint, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<COPQComplaintDumpViewModel?> GetByIdAsync(int id);
        Task<List<PODetailViewModel>> GetPOListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PODetailViewModel?> GetPOByIdAsync(int id);
        Task<OperationResult> CreatePOAsync(PODetail podetail, bool returnCreatedRecord = false);
        Task<OperationResult> UpdatePOAsync(PODetail podetail, bool returnUpdatedRecord = false);
        Task<OperationResult> DeletePOAsync(int id);
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
    }
}
