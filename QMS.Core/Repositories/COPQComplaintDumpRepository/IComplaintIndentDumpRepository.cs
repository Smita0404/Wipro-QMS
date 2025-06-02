using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;

namespace QMS.Core.Repositories.COPQComplaintDumpRepository
{
    public interface IComplaintIndentDumpRepository
    {
        //// ----------------- Complaint Dump ------------------- ////
        Task<List<ComplaintViewModel>> GetListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<OperationResult> CreateAsync(ComplaintDump_Service complaint, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateAsync(ComplaintDump_Service complaint, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteAsync(int id);
        Task<ComplaintViewModel?> GetByIdAsync(int id);
        Task<BulkCreateResult> BulkCreateAsync(List<ComplaintViewModel> listOfData, string fileName, string uploadedBy, string recordType);
        //// ----------------- Complaint Dump ------------------- ////
        


        //// ----------------- Po List ------------------- ////
        Task<List<PendingPoViewModel>> GetPOListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<PendingPoViewModel?> GetPOByIdAsync(int id);
        Task<OperationResult> CreatePOAsync(PendingPo_Service podetail, bool returnCreatedRecord = false);
        Task<OperationResult> UpdatePOAsync(PendingPo_Service podetail, bool returnUpdatedRecord = false);
        Task<OperationResult> DeletePOAsync(int id);
        Task<BulkCreatePOResult> BulkCreatePoAsync(List<PendingPoViewModel> listOfData, string fileName, string uploadedBy, string recordType);

        //// ----------------- Po List ------------------- ////


        //// ----------------- Indent Dump ------------------- ////
        Task<List<IndentDumpViewModel>> GetIndentListAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IndentDumpViewModel?> GetIndentByIdAsync(int id);
        Task<OperationResult> CreateIndentAsync(IndentDump_Service podetail, bool returnCreatedRecord = false);
        Task<OperationResult> UpdateIndentAsync(IndentDump_Service podetail, bool returnUpdatedRecord = false);
        Task<OperationResult> DeleteIndentAsync(int id);
        Task<BulkCreateIndentResult> BulkCreateIndentAsync(List<IndentDumpViewModel> listOfData, string fileName, string uploadedBy, string recordType);

        //// ----------------- Indent Dump ------------------- ////
        Task<List<DropdownOptionViewModel>> GetVendorDropdownAsync();
    }
}
