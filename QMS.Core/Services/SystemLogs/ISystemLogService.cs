using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Services.SystemLogs
{
    public interface ISystemLogService
    {
        bool WriteLog(string strMessage);
    }
}
