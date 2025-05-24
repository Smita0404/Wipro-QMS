using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.COPQComplaintDumpRepository
{
    public interface ICOPQComplaintDumpRepository
    {
        Task<List<COPQComplaintDumpViewModel>> GetListAsync();
        Task<OperationResult> CreateAsync(COPQComplaintDump complaint, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(COPQComplaintDump complaint, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<COPQComplaintDumpViewModel?> GetByIdAsync(int id);
    }
}
