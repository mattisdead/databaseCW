using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis
{
    public class Grade
    {
        [LoadColumn(0)]
        public int studentId;

        [LoadColumn(1)]
        public int subjectId;

        [LoadColumn(2)]
        public float grade;
    }
}
