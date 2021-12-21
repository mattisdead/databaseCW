using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot;

namespace ConsoleApp
{
    internal class Graph
    {
        public static void CreateGradeGraph(double[] grades, string filePath)
        {
            List<double> list = new List<double>();  
            for(int i = 0; i < grades.Length; i++)
            {
                list.Add(i);    
            }
            double[] dataX = list.ToArray(); // lower 
            for (int i = 0; i < dataX.Length; i++)
            {
                Console.WriteLine(dataX[i]);
            }
            double[] dataY = grades; // upper left
            for (int i = 0; i < dataY.Length; i++)
            {
                Console.WriteLine(dataY[i]);
            }
            var plt = new Plot(1000, 1000); // size
            plt.AddScatter(dataX, dataY);
            plt.SaveFig(filePath);
        }
    }
}
