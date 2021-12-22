using Microsoft.ML;
using System;
using System.IO;

namespace Analysis
{
    public class AnalysisProgram
    {
        static readonly string _trainDataPath = "C:/Users/User/projects/databaseCW/Analysis/TrainingData/GradesTableTrain.csv";
        static readonly string _testDataPath = "C:/Users/User/projects/databaseCW/Analysis/TrainingData/GradesTableTest.csv";
        static readonly string _modelPath = "C:/Users/User/projects/databaseCW/Analysis/Analysis/TrainingData/Model.zip";
        static void Main(string[] args)
        { 

        }
        public static float Predict(Grade gradeSample)
        {
            MLContext mlContext = new(seed: 0);

            var model = Train(mlContext, _trainDataPath);

            Evaluate(mlContext, model);

            return TestSinglePrediction(mlContext, model, gradeSample);
        }
        public static ITransformer Train(MLContext mlContext, string dataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<Grade>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "grade")
                 .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "studentIdEncoded", inputColumnName: "studentId"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "subjectIdEncoded", inputColumnName: "subjectId"))
                .Append(mlContext.Transforms.Concatenate("Features", "studentIdEncoded", "subjectIdEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(dataView);

            return model;
        }
        private static void Evaluate(MLContext mlContext, ITransformer model)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<Grade>(_testDataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            //Console.WriteLine($"*       R-Squared Score:      {metrics.RSquared:0.###}");
        }
        private static float TestSinglePrediction(MLContext mlContext, ITransformer model, Grade gradeSample)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<Grade, GradePrediction>(model);

            var prediction = predictionFunction.Predict(gradeSample);

            Console.WriteLine($"Predicted grade is: {prediction.grade:0.####}");
            return prediction.grade;
        }
    }
}