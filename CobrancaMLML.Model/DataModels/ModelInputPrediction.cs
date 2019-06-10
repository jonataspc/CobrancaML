using System;
using System.Collections.Generic;
using System.Text;

namespace CobrancaMLML.Model.DataModels
{
    public class ModelInputPrediction : ModelInput
    {

        public string Identifier { get; set; }

        public float PredictionScore { get; set; }

        public bool Prediction { get; set; }


    }
}
