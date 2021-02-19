using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoProrrogacao
    {
        public int IdContratoRenovacao { get; set; }
        public int IdContrato { get; set; }
        public int IdSituacao { get; set; }
        public int NuRenovacao { get; set; }
        public DateTime DtInicioVigencia { get; set; }
        public DateTime DtFimVigenciaAtual { get; set; }
        public string DsPrazoExecucao { get; set; }
        public DateTime? DtFimVigenciaRenovacao { get; set; }
        public DateTime? DtFimExecucaoRenovacao { get; set; }
        public int? NuPrazoRenovacaoMeses { get; set; }
        public DateTime? DtAplicacao { get; set; }
        public int? IdUsuarioAplicacao { get; set; }
        public bool? IcHistoricoCopiado { get; set; }
        public bool? IcEntregaveisCopiado { get; set; }
        public bool? IcCronogramaFinanceiroCopiado { get; set; }
        public decimal? PcReajuste { get; set; }
        public decimal? VlContratoAntesRenovacao { get; set; }
        public decimal? VlContratoRenovado { get; set; }
        public decimal? VlReajustePeriodo { get; set; }
        public decimal? VlReajusteAcumulado { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
    }
}
