//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using CobrancaMLML.Model.DataModels;
using Microsoft.ML.Trainers.LightGbm;
using Microsoft.ML.Trainers;
using Microsoft.ML.Calibrators;
using System.Numerics;

namespace CobrancaMLML.ConsoleApp
{
    public static class ModelBuilder
    {
        private static string TRAIN_DATA_FILEPATH = @"../../../../../CobrancaML/CobrancaML/data/ds1.tsv";
        public static string MODEL_FILEPATH = @"../../../../CobrancaMLML.Model/MLModel.zip";

        // Create MLContext to be shared across the model creation workflow objects 
        // Set a random seed for repeatable/deterministic results across multiple trainings.
        private static MLContext mlContext = new MLContext(seed: 1);

        public static void CreateModel()
        {
            // Load Data
            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: GetAbsolutePath(TRAIN_DATA_FILEPATH),
                                            hasHeader: true,
                                            separatorChar: '\t',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Build training pipeline
            // Data process configuration with pipeline data transformations 
            var dataProcessPipeline = mlContext.Transforms.Categorical.OneHotEncoding(new[] { new InputOutputColumnPair("profissao", "profissao"), new InputOutputColumnPair("tipo_bem", "tipo_bem"), new InputOutputColumnPair("estado", "estado"), new InputOutputColumnPair("motivo_inad", "motivo_inad") })
                                      .Append(mlContext.Transforms.Categorical.OneHotHashEncoding(new[] { new InputOutputColumnPair("data_entrada_cobranca", "data_entrada_cobranca"), new InputOutputColumnPair("pmt_dt", "pmt_dt"), new InputOutputColumnPair("cidade", "cidade") }))
                                      .Append(mlContext.Transforms.Concatenate("Features", new[] { "profissao", "tipo_bem", "estado", "motivo_inad", "data_entrada_cobranca", "pmt_dt", "cidade", "parcela_em_cobranca", "plano_total_parcs", "percent_evoluc_pgto_plano", "pmt_vl", "dias_atraso", "vlr_total_financiado", "valor_risco", "idade", "qtd_cobranca_anterior", "qtd_pgtos_anterior", "dia_pgto_moda", "ult_dia_pgto", "percentual_honra_pgto", "qtd_fones_cpc", "media_pontuacao_fones" }));

            // Set the training algorithm 
            var trainer = mlContext.BinaryClassification.Trainers.LightGbm(new LightGbmBinaryTrainer.Options() { NumberOfIterations = 200, LearningRate = 0.13136f, NumberOfLeaves = 100, MinimumExampleCountPerLeaf = 10, UseCategoricalSplit = false, HandleMissingValue = true, MinimumExampleCountPerGroup = 10, MaximumCategoricalSplitPointCount = 8, CategoricalSmoothing = 20, L2CategoricalRegularization = 0.5, Booster = new GradientBooster.Options() { L2Regularization = 0, L1Regularization = 0.5 }, LabelColumnName = "pagamento", FeatureColumnName = "Features" });
            var trainingPipeline = dataProcessPipeline.Append(trainer);


           






            // Evaluate quality of Model
            // Cross-Validate with single dataset (since we don't have two datasets, one for training and for evaluate)
            // in order to evaluate and get the model's accuracy metrics
            Console.WriteLine("=============== Cross-validating to get model's accuracy metrics ===============");
            var crossValidationResults = mlContext.BinaryClassification.CrossValidateNonCalibrated(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "pagamento");
            PrintBinaryClassificationFoldsAverageMetrics(crossValidationResults);




            // Train Model
            Console.WriteLine("=============== Training  model ===============");

            var mlModel = trainingPipeline.Fit(trainingDataView);

            //get features' Weights
            //var modelParameters = mlModel.LastTransformer.Model;
            //VBuffer<float> weights = default;

            //modelParameters.SubModel.GetFeatureWeights(ref weights);


            Console.WriteLine("=============== End of training process ===============");





            // Save/persist the trained model to a .ZIP file
            Console.WriteLine($"=============== Saving the model  ===============");
            mlContext.Model.Save(mlModel, trainingDataView.Schema, GetAbsolutePath(MODEL_FILEPATH));
            Console.WriteLine("The model is saved to {0}", GetAbsolutePath(MODEL_FILEPATH));

        }


        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;
        }

        public static void PrintMulticlassClassificationMetrics(MulticlassClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*    Metrics for multi-class classification model   ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"    MacroAccuracy = {metrics.MacroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    MicroAccuracy = {metrics.MicroAccuracy:0.####}, a value between 0 and 1, the closer to 1, the better");
            Console.WriteLine($"    LogLoss = {metrics.LogLoss:0.####}, the closer to 0, the better");
            for (int i = 0; i < metrics.PerClassLogLoss.Count; i++)
            {
                Console.WriteLine($"    LogLoss for class {i + 1} = {metrics.PerClassLogLoss[i]:0.####}, the closer to 0, the better");
            }
            Console.WriteLine($"************************************************************");
        }

        public static void PrintBinaryClassificationMetrics(BinaryClassificationMetrics metrics)
        {
            Console.WriteLine($"************************************************************");
            Console.WriteLine($"*       Metrics for binary classification model      ");
            Console.WriteLine($"*-----------------------------------------------------------");
            Console.WriteLine($"*       Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"*       Auc:      {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"************************************************************");
        }


       

        public static void PrintBinaryClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<BinaryClassificationMetrics>> crossValResults)
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var AccuracyValues = metricsInMultipleFolds.Select(m => m.Accuracy);
            var AccuracyAverage = AccuracyValues.Average();
            var AccuraciesStdDeviation = CalculateStandardDeviation(AccuracyValues);
            var AccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(AccuracyValues);


            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Binary Classification model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average Accuracy:    {AccuracyAverage:0.###}  - Standard deviation: ({AccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({AccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*************************************************************************************************************");
        }

       

        public static void PrintMulticlassClassificationFoldsAverageMetrics(IEnumerable<TrainCatalogBase.CrossValidationResult<MulticlassClassificationMetrics>> crossValResults)
        {
            var metricsInMultipleFolds = crossValResults.Select(r => r.Metrics);

            var microAccuracyValues = metricsInMultipleFolds.Select(m => m.MicroAccuracy);
            var microAccuracyAverage = microAccuracyValues.Average();
            var microAccuraciesStdDeviation = CalculateStandardDeviation(microAccuracyValues);
            var microAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(microAccuracyValues);

            var macroAccuracyValues = metricsInMultipleFolds.Select(m => m.MacroAccuracy);
            var macroAccuracyAverage = macroAccuracyValues.Average();
            var macroAccuraciesStdDeviation = CalculateStandardDeviation(macroAccuracyValues);
            var macroAccuraciesConfidenceInterval95 = CalculateConfidenceInterval95(macroAccuracyValues);

            var logLossValues = metricsInMultipleFolds.Select(m => m.LogLoss);
            var logLossAverage = logLossValues.Average();
            var logLossStdDeviation = CalculateStandardDeviation(logLossValues);
            var logLossConfidenceInterval95 = CalculateConfidenceInterval95(logLossValues);

            var logLossReductionValues = metricsInMultipleFolds.Select(m => m.LogLossReduction);
            var logLossReductionAverage = logLossReductionValues.Average();
            var logLossReductionStdDeviation = CalculateStandardDeviation(logLossReductionValues);
            var logLossReductionConfidenceInterval95 = CalculateConfidenceInterval95(logLossReductionValues);

            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model      ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       Average MicroAccuracy:    {microAccuracyAverage:0.###}  - Standard deviation: ({microAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({microAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average MacroAccuracy:    {macroAccuracyAverage:0.###}  - Standard deviation: ({macroAccuraciesStdDeviation:#.###})  - Confidence Interval 95%: ({macroAccuraciesConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLoss:          {logLossAverage:#.###}  - Standard deviation: ({logLossStdDeviation:#.###})  - Confidence Interval 95%: ({logLossConfidenceInterval95:#.###})");
            Console.WriteLine($"*       Average LogLossReduction: {logLossReductionAverage:#.###}  - Standard deviation: ({logLossReductionStdDeviation:#.###})  - Confidence Interval 95%: ({logLossReductionConfidenceInterval95:#.###})");
            Console.WriteLine($"*************************************************************************************************************");

        }

        public static double CalculateStandardDeviation(IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double standardDeviation = Math.Sqrt(sumOfSquaresOfDifferences / (values.Count() - 1));
            return standardDeviation;
        }

        public static double CalculateConfidenceInterval95(IEnumerable<double> values)
        {
            double confidenceInterval95 = 1.96 * CalculateStandardDeviation(values) / Math.Sqrt((values.Count() - 1));
            return confidenceInterval95;
        }
    }
}
