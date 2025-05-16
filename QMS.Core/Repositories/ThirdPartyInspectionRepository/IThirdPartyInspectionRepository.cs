using QMS.Core.Models;
using QMS.Core.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.ThirdPartyInspectionRepository
{
    public interface IThirdPartyInspectionRepository
    {
        Task<List<ThirdPartyInspectionViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);

        Task<OperationResult> CreateAsync(ThirdPartyInspection inspection, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(ThirdPartyInspection inspection, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int inspectionId,string UpdatedBy);
        Task<ThirdPartyInspection> GetByIdAsync(int inspectionId);
        Task<bool> CheckDuplicate(string projectName, int inspectionId);
        Task UpdateAttachmentPathAsync(ThirdPartyInspection inspection);
        Task UpdateAttachmentPath(ThirdPartyInspection inspection);
        Task<OperationResult> RemoveAttachmentAsync(int inspectionId, string filePath, string updatedBy);

    }
}
