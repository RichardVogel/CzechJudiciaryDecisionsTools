using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CzechJudiciaryDecisionsTools.Models
{
    public class SupremeCourtDecision
    {
        public string Court { get; set; }
        public string CaseNumber { get; set; }
        public DateTime DecisionDate { get; set; }
        public string DecisionType { get; set; }
        public string Keywords { get; set; }
        public string RelevantLaws { get; set; }
        public string DecisionCategory { get; set; }
        public string TextFileName { get; set;}
    }

}
