using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XYKernel.OS.Common.Models.Mixed.AlgoRule
{
    public class ClassHoursMaxConcurrencyInSelectedTimeRule:BaseRule
    {
        public int[] ID { get; set; }

        public int MaxConcurrencyNumber { get; set; }

        public List<DayPeriodModel> Times { get; set; }
    }
}
