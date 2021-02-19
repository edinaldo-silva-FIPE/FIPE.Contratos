using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoAditivo
    {
        public ContratoAditivo()
        {
            ContratoCronogramaFinanceiroHistorico = new HashSet<ContratoCronogramaFinanceiroHistorico>();
            ContratoEntregavelHistórico = new HashSet<ContratoEntregavelHistórico>();
        }

        public int IdContratoAditivo { get; set; }
        public int IdContrato { get; set; }
        public short? IdTipoAditivo { get; set; }
        public int IdSituacao { get; set; }
        public short NuAditivo { get; set; }
        public DateTime DtInicio { get; set; }
        public DateTime DtFim { get; set; }
        public DateTime? DtIniExecucaoAditivo { get; set; }
        public DateTime? DtFimAditivada { get; set; }
        public decimal VlContrato { get; set; }
        public decimal? VlContratoAditivado { get; set; }
        public DateTime DtCriacao { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public string DsAditivo { get; set; }
        public int IdProposta { get; set; }
        public bool? IcAditivoValor { get; set; }
        public bool? IcAditivoEscopo { get; set; }
        public bool? IcAditivoData { get; set; }
        public decimal? VlAditivo { get; set; }
        public int? IdUsuarioAplicacao { get; set; }
        public DateTime? DtAplicacao { get; set; }
        public bool? IcAditivoRetRat { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Proposta IdPropostaNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual TipoAditivo IdTipoAditivoNavigation { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroHistorico> ContratoCronogramaFinanceiroHistorico { get; set; }
        public virtual ICollection<ContratoEntregavelHistórico> ContratoEntregavelHistórico { get; set; }
    }
}
