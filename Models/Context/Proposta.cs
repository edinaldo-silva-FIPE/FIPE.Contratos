using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Proposta
    {
        public Proposta()
        {
            ContratoAditivo = new HashSet<ContratoAditivo>();
            PropostaCliente = new HashSet<PropostaCliente>();
            PropostaComentario = new HashSet<PropostaComentario>();
            PropostaCoordenador = new HashSet<PropostaCoordenador>();
            PropostaDocs = new HashSet<PropostaDocs>();
            PropostaHistorico = new HashSet<PropostaHistorico>();
        }

        public int IdProposta { get; set; }
        public int IdSituacao { get; set; }
        public int? IdTipoProposta { get; set; }
        public int? IdOportunidade { get; set; }
        public int? IdTema { get; set; }
        public string DsApelidoProposta { get; set; }
        public DateTime? DtProposta { get; set; }
        public DateTime? DtValidadeProposta { get; set; }
        public DateTime? DtLimiteEnvioProposta { get; set; }
        public string DsAssunto { get; set; }
        public string DsObjeto { get; set; }
        public short? NuPrazoExecucao { get; set; }
        public decimal? VlProposta { get; set; }
        public decimal? NuPrazoEstimadoMes { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public DateTime DtCriacao { get; set; }
        public int? IdUsuarioUltimaAlteracao { get; set; }
        public DateTime? DtUltimaAlteracao { get; set; }
        public DateTime? DtAssinaturaContrato { get; set; }
        public DateTime? DtAutorizacaoInicio { get; set; }
        public DateTime? DtLimiteEntregaProposta { get; set; }
        public short? IdFundamento { get; set; }
        public decimal? VlContrato { get; set; }
        public bool? RenovacaoAutomatica { get; set; }
        public bool? OrdemInicio { get; set; }
        public bool? Reajustes { get; set; }
        public short? IdTipoReajuste { get; set; }
        public string DsObservacao { get; set; }
        public int? IdContrato { get; set; }
        public string DsAditivo { get; set; }
        public bool? IcRitoSumario { get; set; }
        public DateTime? DtNovoFimVigencia { get; set; }
        public short? IdTipoAditivo { get; set; }
        public int? IdTipoOportunidade { get; set; }
        public decimal? VlContratoComAditivo { get; set; }
        public bool? IcContratantesValidos { get; set; }
        public short? IdUnidadeTempo { get; set; }
        public short? IdUnidadeTempoJuridico { get; set; }
        public short? NuPrazoExecucaoJuridico { get; set; }
        public string NuContratoCliente { get; set; }
        public decimal? NuPrazoEstimadoMesJuridico { get; set; }
        public bool? IcInformacoesIncompletas { get; set; }
        public bool? IcAditivoData { get; set; }
        public bool? IcAditivoEscopo { get; set; }
        public bool? IcAditivoValor { get; set; }
        public decimal? VlAditivo { get; set; }
        public bool? IcAditivoRetRat { get; set; }
        public bool? IcAditivoAnalisado { get; set; }
        public short  IdComprovacaoValor { get; set; }
        public string ComprovacaoValorJustifica { get; set; }



        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual FundamentoContratacao IdFundamentoNavigation { get; set; }
        public virtual Oportunidade IdOportunidadeNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual Tema IdTemaNavigation { get; set; }
        public virtual TipoAditivo IdTipoAditivoNavigation { get; set; }
        public virtual TipoOportunidade IdTipoOportunidadeNavigation { get; set; }
        public virtual TipoProposta IdTipoPropostaNavigation { get; set; }
        public virtual IndiceReajuste IdTipoReajusteNavigation { get; set; }
        public virtual UnidadeDeTempo IdUnidadeTempoJuridicoNavigation { get; set; }
        public virtual UnidadeDeTempo IdUnidadeTempoNavigation { get; set; }
        public virtual Usuario IdUsuarioCriacaoNavigation { get; set; }
        public virtual Contrato Contrato { get; set; }
        public virtual ICollection<ContratoAditivo> ContratoAditivo { get; set; }
        public virtual ICollection<PropostaCliente> PropostaCliente { get; set; }
        public virtual ICollection<PropostaComentario> PropostaComentario { get; set; }
        public virtual ICollection<PropostaCoordenador> PropostaCoordenador { get; set; }
        public virtual ICollection<PropostaDocs> PropostaDocs { get; set; }
        public virtual ICollection<PropostaHistorico> PropostaHistorico { get; set; }
    }
}
