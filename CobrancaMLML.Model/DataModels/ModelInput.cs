//*****************************************************************************************
//*                                                                                       *
//* This is an auto-generated file by Microsoft ML.NET CLI (Command-Line Interface) tool. *
//*                                                                                       *
//*****************************************************************************************

using Microsoft.ML.Data;

namespace CobrancaMLML.Model.DataModels
{
    public class ModelInput
    {
        [ColumnName("data_entrada_cobranca"), LoadColumn(0)]
        public string Data_entrada_cobranca { get; set; }


        [ColumnName("parcela_em_cobranca"), LoadColumn(1)]
        public float Parcela_em_cobranca { get; set; }


        [ColumnName("plano_total_parcs"), LoadColumn(2)]
        public float Plano_total_parcs { get; set; }


        [ColumnName("percent_evoluc_pgto_plano"), LoadColumn(3)]
        public float Percent_evoluc_pgto_plano { get; set; }


        [ColumnName("pmt_vl"), LoadColumn(4)]
        public float Pmt_vl { get; set; }


        [ColumnName("pmt_dt"), LoadColumn(5)]
        public string Pmt_dt { get; set; }


        [ColumnName("dias_atraso"), LoadColumn(6)]
        public float Dias_atraso { get; set; }


        [ColumnName("profissao"), LoadColumn(7)]
        public string Profissao { get; set; }


        [ColumnName("tipo_bem"), LoadColumn(8)]
        public string Tipo_bem { get; set; }


        [ColumnName("vlr_total_financiado"), LoadColumn(9)]
        public float Vlr_total_financiado { get; set; }


        [ColumnName("valor_risco"), LoadColumn(10)]
        public float Valor_risco { get; set; }


        [ColumnName("cidade"), LoadColumn(11)]
        public string Cidade { get; set; }


        [ColumnName("estado"), LoadColumn(12)]
        public string Estado { get; set; }


        [ColumnName("idade"), LoadColumn(13)]
        public float Idade { get; set; }


        [ColumnName("motivo_inad"), LoadColumn(14)]
        public string Motivo_inad { get; set; }


        [ColumnName("qtd_cobranca_anterior"), LoadColumn(15)]
        public float Qtd_cobranca_anterior { get; set; }


        [ColumnName("qtd_pgtos_anterior"), LoadColumn(16)]
        public float Qtd_pgtos_anterior { get; set; }


        [ColumnName("dia_pgto_moda"), LoadColumn(17)]
        public float Dia_pgto_moda { get; set; }


        [ColumnName("ult_dia_pgto"), LoadColumn(18)]
        public float Ult_dia_pgto { get; set; }


        [ColumnName("percentual_honra_pgto"), LoadColumn(19)]
        public float Percentual_honra_pgto { get; set; }


        [ColumnName("qtd_fones_cpc"), LoadColumn(20)]
        public float Qtd_fones_cpc { get; set; }


        [ColumnName("media_pontuacao_fones"), LoadColumn(21)]
        public float Media_pontuacao_fones { get; set; }


        [ColumnName("pagamento"), LoadColumn(22)]
        public bool Pagamento { get; set; }


    }
}
