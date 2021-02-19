using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoCronogramaFinanceiroTemporaria
    {
        public ContratoCronogramaFinanceiroTemporaria()
        {
            ContratoParcelaEntregavelTemporaria = new HashSet<ContratoParcelaEntregavelTemporaria>();
        }

        public int IdContratoCronFinanceiro { get; set; }
        public int IdContrato { get; set; }
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
        public int? IdFrente { get; set; }
        public bool? IcAtraso { get; set; }
        public string DsObservacao { get; set; }
        public int? IdParcela { get; set; }

        public virtual ContratoCliente IdContratoClienteNavigation { get; set; }
        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Frente IdFrenteNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual ICollection<ContratoParcelaEntregavelTemporaria> ContratoParcelaEntregavelTemporaria { get; set; }
    }
}
