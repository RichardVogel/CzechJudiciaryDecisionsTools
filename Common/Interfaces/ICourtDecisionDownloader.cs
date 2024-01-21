using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Common.Interfaces
{
    public interface ICourtDecisionDownloader
    {
        Task<(bool Success, string Message)> DownloadDecisionByDateAsync(DateTime date);
    }
}
