using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoReajuste
    {
        public ContratoReajuste()
        {
            ContratoCronogramaFinanceiroHistorico = new HashSet<ContratoCronogramaFinanceiroHistorico>();
            ContratoReajusteHistoricoCronogramaFinanceiro = new HashSet<ContratoReajusteHistoricoCronogramaFinanceiro>();
        }

        public int IdContratoReajuste { get; set; }
        public int IdContrato { get; set; }
        public short? NuReajuste { get; set; }
        public int IdSituacao { get; set; }
        public DateTime? DtAplicacao { get; set; }
        public DateTime? DtReajuste { get; set; }
        public DateTime? DtProxReajuste { get; set; }
        public decimal VlContratoAntesReajuste { get; set; }
        public decimal? PcReajuste { get; set; }
        public decimal? VlContratoReajustado { get; set; }
        public decimal? VlReajuste { get; set; }
        public decimal? VlReajusteAcumulado { get; set; }
        public bool? IcHistoricoCopiado { get; set; }
        public bool? IcCronogramaFinanceiroCopiado { get; set; }
        public int? IdUsuarioAplicacao { get; set; }
        public short? IdIndiceReajuste { get; set; }
        public bool? IcReajuste { get; set; }
        public bool? IcReajusteConcluido { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual IndiceReajuste IdIndiceReajusteNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroHistorico> ContratoCronogramaFinanceiroHistorico { get; set; }
        public virtual ICollection<ContratoReajusteHistoricoCronogramaFinanceiro> ContratoReajusteHistoricoCronogramaFinanceiro { get; set; }
    }
}
