using QMS.Core.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using QMS.Core.DatabaseContext.Shared;
using QMS.Core.Models;

namespace QMS.Core.Repositories.Shared;

public class SqlTableRepository : ISqlTableRepository
{
    public readonly QMSDbContext _dbContext;

    public SqlTableRepository(QMSDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OperationResult> CreateAsync<TEntity>(SqlTable record, bool returnCreatedRecord = false) where TEntity : SqlTable
    {
        try
        {
            record.Deleted = false;
            _dbContext.Entry(record).State = EntityState.Added;
            _dbContext.Add(record);
            var operationSuccess = await _dbContext.SaveChangesAsync() == 1;
            var result = new OperationResult
            {
                Success = operationSuccess,
                ObjectId = record.Id
            };
            if (returnCreatedRecord)
            {
                result.Payload = await GetByIdAsync<TEntity>(record.Id);
            }

            if (!result.Success)
            {
                result.Message = "Unable To Create New Record.";
            }

            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateConcurrencyException.Message,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<OperationResult> BulkCreateAsync<TEntity>(List<TEntity> list) where TEntity : SqlTable
    {
        try
        {
            foreach(var record in list)
            {
                record.Deleted = false;
                _dbContext.Entry(record).State = EntityState.Added;
                _dbContext.Add(record);
            }
            var operationSuccess = await _dbContext.SaveChangesAsync() == list.Count;
            var result = new OperationResult
            {
                Success = operationSuccess,
            };

            if (!result.Success)
            {
                result.Message = "Unable To Create New Records.";
            }

            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                Success = false,
                Message = dbUpdateConcurrencyException.Message,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<OperationResult> UpdateAsync<TEntity>(SqlTable modifiedRecord, bool returnUpdatedRecord = false) where TEntity : SqlTable
    {
        if (modifiedRecord.Id <= 0)
        {
            return new OperationResult
            {
                Success = false,
                Message = "Record Cannot Be Edited.",
                ObjectId = modifiedRecord.Id
            };
        }

        var currentRecord = await _dbContext.Set<TEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == modifiedRecord.Id);
        if (currentRecord is null)
        {
            return new OperationResult
            {
                Message = "Unable To Find This Record.",
                Success = false
            };
        }

        try
        {
            _dbContext.Entry(currentRecord).CurrentValues.SetValues(modifiedRecord);
            _dbContext.Entry(currentRecord).State = EntityState.Modified;
            var operationSuccess = await _dbContext.SaveChangesAsync() == 1;
            var result = new OperationResult
            {
                ObjectId = modifiedRecord.Id,
                Success = operationSuccess
            };
            if (returnUpdatedRecord)
            {
                result.Payload = await GetByIdAsync<TEntity>(modifiedRecord.Id);
            }

            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                ObjectId = modifiedRecord.Id,
                Message = dbUpdateConcurrencyException.Message,
                Success = false,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = modifiedRecord.Id,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = modifiedRecord.Id,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<OperationResult> DeleteAsync<TEntity>(int id) where TEntity : SqlTable
    {
        var record = await _dbContext.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (record is null)
        {
            return new OperationResult
            {
                Message = "Unable To Find This Record.",
                Success = false
            };
        }

        try
        {
            var recordId = record.Id;
            var local = await _dbContext.Set<TEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == recordId);
            if (local is null)
            {
                return new OperationResult
                {
                    Message = "Unable To Find This Record.",
                    Success = false
                };
            }

            record.Deleted = true;
            _dbContext.Entry(local).CurrentValues.SetValues(record);
            _dbContext.Entry(local).State = EntityState.Modified;
            var operationSuccess = await _dbContext.SaveChangesAsync() == 1;
            var result = new OperationResult
            {
                Success = operationSuccess,
                ObjectId = recordId
            };
            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateConcurrencyException.Message,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<OperationResult> DeletePermanentlyAsync<TEntity>(int id) where TEntity : SqlTable
    {
        var record = await _dbContext.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (record is null)
        {
            return new OperationResult
            {
                Message = "Unable To Find This Record.",
                Success = false
            };
        }

        try
        {
            var recordId = record.Id;
            var local = _dbContext.Set<TEntity>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(record.Id));
            if (local is not null)
            {
                _dbContext.Entry(local).State = EntityState.Detached;
            }

            _dbContext.Entry(record).State = EntityState.Deleted;
            var operationSuccess = await _dbContext.SaveChangesAsync() == 1;
            var result = new OperationResult
            {
                Success = operationSuccess,
                ObjectId = recordId
            };
            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateConcurrencyException.Message,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<OperationResult> RestoreAsync<TEntity>(int id) where TEntity : SqlTable
    {
        var record = await _dbContext.Set<TEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id);
        if (record is null)
        {
            return new OperationResult
            {
                Message = "Unable To Find This Record.",
                Success = false
            };
        }

        try
        {
            var recordId = record.Id;
            var local = await _dbContext.Set<TEntity>()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
            if (local is null)
            {
                return new OperationResult
                {
                    Message = "Unable To Find This Record.",
                    Success = false
                };
            }

            record.Deleted = false;
            _dbContext.Entry(local).CurrentValues.SetValues(record);
            _dbContext.Entry(local).State = EntityState.Modified;
            var operationSuccess = await _dbContext.SaveChangesAsync() == 1;
            var result = new OperationResult
            {
                Success = operationSuccess,
                ObjectId = recordId
            };
            return result;
        }
        catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateConcurrencyException.Message,
                Exception = dbUpdateConcurrencyException
            };
        }
        catch (DbUpdateException dbUpdateException)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = dbUpdateException.Message,
                Exception = dbUpdateException
            };
        }
        catch (Exception e)
        {
            return new OperationResult
            {
                Success = false,
                ObjectId = record.Id,
                Message = e.Message,
                Exception = e
            };
        }
    }

    public async Task<TEntity?> GetByIdAsync<TEntity>(int id) where TEntity : SqlTable
    {
        var query = _dbContext.Set<TEntity>().IgnoreQueryFilters();
        return await query.FirstOrDefaultAsync(x => x.Id == id && x.Deleted == false);
    }

    public async Task<TEntity?> GetByTextAsync<TEntity>(int id) where TEntity : SqlTable
    {
        var query = _dbContext.Set<TEntity>().IgnoreQueryFilters();
        return await query.FirstOrDefaultAsync(x => x.Id == id && x.Deleted == false);
    }
}
