using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Frente
    {
        public Frente()
        {
            ContratoCronogramaFinanceiro = new HashSet<ContratoCronogramaFinanceiro>();
            ContratoCronogramaFinanceiroHistorico = new HashSet<ContratoCronogramaFinanceiroHistorico>();
            ContratoCronogramaFinanceiroTemporaria = new HashSet<ContratoCronogramaFinanceiroTemporaria>();
            ContratoEntregavel = new HashSet<ContratoEntregavel>();
            ContratoEntregavelTemporaria = new HashSet<ContratoEntregavelTemporaria>();
            FrenteCoordenador = new HashSet<FrenteCoordenador>();
        }

        public int IdFrente { get; set; }
        public string NmFrente { get; set; }
        public int IdContrato { get; set; }
        public int CdFrente { get; set; }
        public string CdFrenteTexto { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiro> ContratoCronogramaFinanceiro { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroHistorico> ContratoCronogramaFinanceiroHistorico { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroTemporaria> ContratoCronogramaFinanceiroTemporaria { get; set; }
        public virtual ICollection<ContratoEntregavel> ContratoEntregavel { get; set; }
        public virtual ICollection<ContratoEntregavelTemporaria> ContratoEntregavelTemporaria { get; set; }
        public virtual ICollection<FrenteCoordenador> FrenteCoordenador { get; set; }
    }
}
