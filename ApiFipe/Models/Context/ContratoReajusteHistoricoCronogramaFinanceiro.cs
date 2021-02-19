using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoReajusteHistoricoCronogramaFinanceiro
    {
        public int IdHistoricoCronogramaFinanceiro { get; set; }
        public int IdContrato { get; set; }
        public int IdCliente { get; set; }
        public int? IdContratoReajuste { get; set; }
        public int IdContratoCliente { get; set; }
        public short NuParcela { get; set; }
        public decimal VlParcela { get; set; }
        public DateTime? DtFaturamento { get; set; }
        public string CdIss { get; set; }
        public string CdParcela { get; set; }
        public int IdSituacao { get; set; }
        public string DsTextoCorpoNf { get; set; }
        public string NuNotaFiscal { get; set; }
        public DateTime? DtNotaFiscal { get; set; }

        public virtual ContratoReajuste IdContratoReajusteNavigation { get; set; }
    }
}
