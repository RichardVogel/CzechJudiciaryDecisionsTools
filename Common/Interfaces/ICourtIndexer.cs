using CzechJudiciaryDecisionsTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Common.Interfaces
{
    public interface ICourtIndexer
    {
        public Task<List<SupremeCourtDecision>> IndexDecisionsAsync();
    }

}
