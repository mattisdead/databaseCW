using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis
{
    internal class GradePrediction
    {
        [ColumnName("Score")]
        public float grade;
    }
}
