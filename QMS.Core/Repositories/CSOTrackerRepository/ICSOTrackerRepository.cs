using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.CSOTrackerRepository
{
    public interface ICSOTrackerRepository
    {
        Task<List<CSOTrackerViewModel>> GetListAsync();
        Task<CSOTracker?> GetByIdAsync(int id);
        Task<OperationResult> CreateAsync(CSOTracker entity, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(CSOTracker entity, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
    }
}
