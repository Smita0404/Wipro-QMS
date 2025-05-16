using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QMS.Core.Repositories.VendorRepository.VendorRepository;

namespace QMS.Core.Repositories.BisProjectTracRepository
{
    public interface IBisProjectTracRepository
    {
        Task<List<BisProjectTracViewModel>> GetListAsync();
        Task<OperationResult> CreateAsync(BisProject_Tracker bisProject, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(BisProject_Tracker bisProject, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int Id);
        Task<BisProjectTracViewModel?> GetByIdAsync(int id);
        Task<bool> CheckDuplicate(string searchText, int id);
    }
}
