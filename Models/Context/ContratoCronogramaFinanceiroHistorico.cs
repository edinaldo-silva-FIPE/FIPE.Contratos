using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoCronogramaFinanceiroHistorico
    {
        public long IdContratoCrongramaHistorico { get; set; }
        public int? IdContratoAditivo { get; set; }
        public int? IdContratoCliente { get; set; }
        public short NuParcela { get; set; }
        public decimal VlParcela { get; set; }
        public DateTime? DtFaturamento { get; set; }
        public string CdIss { get; set; }
        public string CdParcela { get; set; }
        public int IdSituacao { get; set; }
        public string DsTextoCorpoNf { get; set; }
        public string NuNotaFiscal { get; set; }
        public DateTime? DtNotaFiscal { get; set; }
        public int? IdFrente { get; set; }
        public string NuEntregaveis { get; set; }
        public int? IdContratoReajuste { get; set; }
        public string DsObservacao { get; set; }

        public virtual ContratoAditivo IdContratoAditivoNavigation { get; set; }
        public virtual ContratoCliente IdContratoClienteNavigation { get; set; }
        public virtual ContratoReajuste IdContratoReajusteNavigation { get; set; }
        public virtual Frente IdFrenteNavigation { get; set; }
    }
}
