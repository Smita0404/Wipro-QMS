using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.NPITrackerRepository
{
    public interface INPITrackerRepository
    {
        Task<List<NPI_TarcViewModel>> GetListAsync();
        Task<OperationResult> CreateAsync(NPITracker npiTracker, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(NPITracker npiTracker, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int Id);
        Task<NPI_TarcViewModel?> GetByIdAsync(int id);
        Task<bool> CheckDuplicate(string searchText, int id);
    }
}
