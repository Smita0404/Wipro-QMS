using QMS.Core.DatabaseContext.Shared;
using QMS.Core.Models;

namespace QMS.Core.Repositories.Shared;

public interface ISqlTableRepository
{
    Task<OperationResult> CreateAsync<TEntity>(SqlTable record, bool returnCreatedRecord = false) where TEntity : SqlTable;
    Task<OperationResult> BulkCreateAsync<TEntity>(List<TEntity> list) where TEntity : SqlTable;
    Task<OperationResult> UpdateAsync<TEntity>(SqlTable modifiedRecord, bool returnUpdatedRecord = false) where TEntity : SqlTable;
    Task<OperationResult> DeleteAsync<TEntity>(int id) where TEntity : SqlTable;
    Task<OperationResult> DeletePermanentlyAsync<TEntity>(int id) where TEntity : SqlTable;
    Task<OperationResult> RestoreAsync<TEntity>(int id) where TEntity : SqlTable;
    Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : SqlTable;
}
