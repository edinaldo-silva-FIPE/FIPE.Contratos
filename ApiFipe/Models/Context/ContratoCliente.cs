using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoCliente
    {
        public ContratoCliente()
        {
            ContratoContatos = new HashSet<ContratoContatos>();
            ContratoCronogramaFinanceiro = new HashSet<ContratoCronogramaFinanceiro>();
            ContratoCronogramaFinanceiroHistorico = new HashSet<ContratoCronogramaFinanceiroHistorico>();
            ContratoCronogramaFinanceiroTemporaria = new HashSet<ContratoCronogramaFinanceiroTemporaria>();
            ContratoEntregavel = new HashSet<ContratoEntregavel>();
            ContratoEntregavelHistórico = new HashSet<ContratoEntregavelHistórico>();
            ContratoEntregavelTemporaria = new HashSet<ContratoEntregavelTemporaria>();
        }

        public int IdContratoCliente { get; set; }
        public int IdContrato { get; set; }
        public int IdCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string NmFantasia { get; set; }
        public bool? IcPagador { get; set; }
        public int? NuContratante { get; set; }
        public bool? IcSomentePagador { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual ICollection<ContratoContatos> ContratoContatos { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiro> ContratoCronogramaFinanceiro { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroHistorico> ContratoCronogramaFinanceiroHistorico { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroTemporaria> ContratoCronogramaFinanceiroTemporaria { get; set; }
        public virtual ICollection<ContratoEntregavel> ContratoEntregavel { get; set; }
        public virtual ICollection<ContratoEntregavelHistórico> ContratoEntregavelHistórico { get; set; }
        public virtual ICollection<ContratoEntregavelTemporaria> ContratoEntregavelTemporaria { get; set; }
    }
}
