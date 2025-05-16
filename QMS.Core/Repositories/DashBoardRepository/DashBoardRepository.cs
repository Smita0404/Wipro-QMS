using Microsoft.EntityFrameworkCore;
using QMS.Core.Repositories.DashBordRepository;
using QMS.Core.DatabaseContext;
using QMS.Core.Models;
using QMS.Core.Repositories.Shared;
using QMS.Core.Services.SystemLogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Repositories.DashBoardRepository
{
    public class DashBoardRepository : SqlTableRepository, IDashBoardRepository
    {
        public new readonly QMSDbContext _dbContext;
        private readonly ISystemLogService _systemLogService;

        public DashBoardRepository(QMSDbContext dbContext,
            ISystemLogService systemLogService) : base(dbContext)
        {
            _dbContext = dbContext;
            _systemLogService = systemLogService;

        }

        //public async Task<List<Rel_Ver_DetailViewModel>> Get_Rele_Ver_DetailAsync()
        //{
        //    try
        //    {
        //        var result = await (from pr in _dbContext.ReleaseVersionDetails
        //                            where pr.Deleted == false // Add this condition
        //                            select new Rel_Ver_DetailViewModel
        //                            {
        //                                Id = pr.Id,
        //                                Date = pr.Date,
        //                                Version = pr.Version,
        //                                ReleaseNote = pr.ReleaseNote
        //                            }).ToListAsync();
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        _systemLogService.WriteLog(ex.Message);
        //        throw;
        //    }
        //}
    }
}
