using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Parametro
    {
        public int IdParametro { get; set; }
        public string DsPrazoPagto { get; set; }
        public string NuBanco { get; set; }
        public string NuAgencia { get; set; }
        public string NuConta { get; set; }
        public string DsTextoCorpoNf { get; set; }
        public short? NuDiasEntregaveis { get; set; }
        public short? NuDiasFaturamento { get; set; }
        public string EmailsNotificacao { get; set; }
        public int? NuDiasReajuste { get; set; }
        public int? NuDiasEncerramentoContrato { get; set; }
        public decimal? NuPercentualOverhead { get; set; }
        public int? NuDiasRenovacao { get; set; }
    }
}
