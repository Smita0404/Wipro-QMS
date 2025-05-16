using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QMS.Core.Services.SystemLogs
{
    public class SystemLogService : ISystemLogService
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public SystemLogService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public bool WriteLog(string strMessage)
        {
            try
            {
                var folderName = Path.Combine(_hostingEnvironment.WebRootPath, "Log");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", pathToSave, "Logs"), FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter((Stream)objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
