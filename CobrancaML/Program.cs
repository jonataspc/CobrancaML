using CobrancaMLML.Model.DataModels;
using CsvHelper;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace CobrancaML
{
    class Program
    {

        private static string BaseDatasetsRelativePath = @"..\..\..\data";
        static void Main(string[] args)
        {
            //set current culture (e.g format decimal separators) 
            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            //run and save predictions
            PredictOpenCases();
        }

        //

        private static void PredictOpenCases()
        {

            // Load the model
            MLContext mlContext = new MLContext();

            ITransformer mlModel = mlContext.Model.Load("MLModel.zip", out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            //input csv containing open cases
            var pathInputCsvFile = GetAbsolutePath($@"{BaseDatasetsRelativePath}\OpenCases.csv");

            //output csv containg the predictions result
            var pathOutputCsvFile = GetAbsolutePath($@"{BaseDatasetsRelativePath}\OpenCasesOutput.csv");


            IEnumerable<ModelInputPrediction> openCases = CsvToList(pathInputCsvFile);

            foreach (var item in openCases)
            {
                //predict the result
                ModelOutput result = predEngine.Predict(item);

                Console.WriteLine($"Debt Id: {item.Identifier} | Prediction: {result.Prediction} | Score: {result.Score.ToString("0.0000000000")} | Probability: {result.Probability.ToString("0.0000000000")} ");

                //set the score in the object 
                item.PredictionScore = result.Score;
                item.Prediction = result.Prediction;
                item.PredictionProbability = result.Probability;
            }

            //export results to the output file
            using (var writer = new StreamWriter(pathOutputCsvFile))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(openCases);
            }


            Console.WriteLine("Prediction finished, results saved.");
            Console.Read();
        }

        private static IEnumerable<ModelInputPrediction> CsvToList(string pathCsvFile)
        {
            List<ModelInputPrediction> retLst = new List<ModelInputPrediction>();

            using (StreamReader oReader = new StreamReader(pathCsvFile))
            {
                var linesRead = 0;
                while (!oReader.EndOfStream)
                {
                    var txtLine = oReader.ReadLine();
                    var fields = txtLine.Split(';');

                    //skip first line
                    if (linesRead != 0)
                    {

                        ModelInputPrediction oInput = new ModelInputPrediction()
                        {
                            Data_entrada_cobranca = fields[0],
                            Parcela_em_cobranca = ConvertValue(fields[1]),
                            Plano_total_parcs = ConvertValue(fields[2]),
                            Percent_evoluc_pgto_plano = ConvertValue(fields[3]),
                            Pmt_vl = ConvertValue(fields[4]),
                            Pmt_dt = fields[5],
                            Dias_atraso = ConvertValue(fields[6]),
                            Profissao = fields[7],
                            Tipo_bem = fields[8],
                            Vlr_total_financiado = ConvertValue(fields[9]),
                            Valor_risco = ConvertValue(fields[10]),
                            Cidade = fields[11],
                            Estado = fields[12],
                            Idade = ConvertValue(fields[13]),
                            Motivo_inad = fields[14],
                            Qtd_cobranca_anterior = ConvertValue(fields[15]),
                            Qtd_pgtos_anterior = ConvertValue(fields[16]),
                            Dia_pgto_moda = ConvertValue(fields[17]),
                            Ult_dia_pgto = ConvertValue(fields[18]),
                            Percentual_honra_pgto = ConvertValue(fields[19]),
                            Qtd_fones_cpc = ConvertValue(fields[20]),
                            Media_pontuacao_fones = ConvertValue(fields[21]),
                            Identifier = fields[23]
                        };

                        retLst.Add(oInput);
                    }

                    linesRead++;
                }
            }

            return retLst;
        }

        private static float ConvertValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0;
            }

            return float.Parse(input, CultureInfo.InvariantCulture);
        }

        public static string GetAbsolutePath(string relativePath)
        {
            FileInfo _dataRoot = new FileInfo(typeof(Program).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;

            string fullPath = Path.Combine(assemblyFolderPath, relativePath);

            return fullPath;

        }

    }
}
