using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Contrato
    {
        public Contrato()
        {
            ContratoAditivo = new HashSet<ContratoAditivo>();
            ContratoCliente = new HashSet<ContratoCliente>();
            ContratoComentario = new HashSet<ContratoComentario>();
            ContratoContatos = new HashSet<ContratoContatos>();
            ContratoCoordenador = new HashSet<ContratoCoordenador>();
            ContratoCronogramaFinanceiro = new HashSet<ContratoCronogramaFinanceiro>();
            ContratoCronogramaFinanceiroTemporaria = new HashSet<ContratoCronogramaFinanceiroTemporaria>();
            ContratoDoc = new HashSet<ContratoDoc>();
            ContratoDocPrincipal = new HashSet<ContratoDocPrincipal>();
            ContratoDocsAcompanhaNf = new HashSet<ContratoDocsAcompanhaNf>();
            ContratoEntregavel = new HashSet<ContratoEntregavel>();
            ContratoEntregavelTemporaria = new HashSet<ContratoEntregavelTemporaria>();
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
            ContratoHistorico = new HashSet<ContratoHistorico>();
            ContratoProrrogacao = new HashSet<ContratoProrrogacao>();
            ContratoReajuste = new HashSet<ContratoReajuste>();
            Frente = new HashSet<Frente>();
            Proposta = new HashSet<Proposta>();
        }

        public int IdContrato { get; set; }
        public int IdProposta { get; set; }
        public int IdSituacao { get; set; }
        public short IdFundamento { get; set; }
        public short? IdIndiceReajuste { get; set; }
        public bool? IcOrdemInicio { get; set; }
        public bool? IcReajuste { get; set; }
        public bool? IcRenovacaoAutomatica { get; set; }
        public string NuContratoCliente { get; set; }
        public string DsApelido { get; set; }
        public string DsAssunto { get; set; }
        public string DsObjeto { get; set; }
        public decimal VlContrato { get; set; }
        public string DsPrazoExecucao { get; set; }
        public DateTime? DtAssinatura { get; set; }
        public DateTime? DtInicio { get; set; }
        public DateTime? DtFim { get; set; }
        public DateTime? DtProxReajuste { get; set; }
        public int IdUsuarioCriacao { get; set; }
        public DateTime DtCriacao { get; set; }
        public string NuCentroCusto { get; set; }
        public string CdIss { get; set; }
        public int? IdUsuarioUltimaAlteracao { get; set; }
        public DateTime? DtUltimaAlteracao { get; set; }
        public int? IdTema { get; set; }
        public DateTime? DtRenovacao { get; set; }
        public string DsObservacao { get; set; }
        public int? IdFormaPagamento { get; set; }
        public short? IdTipoEntregaDocumento { get; set; }
        public short? IdTipoCobranca { get; set; }
        public int? IdTipoApresentacaoRelatorio { get; set; }
        public string IcFatAprovEntregavel { get; set; }
        public string IcFatPedidoEmpenho { get; set; }
        public string DsPrazoPagamento { get; set; }
        public string NuBanco { get; set; }
        public string NuAgencia { get; set; }
        public string NuConta { get; set; }
        public string DsTextoCorpoNf { get; set; }
        public bool? IcFrenteUnica { get; set; }
        public bool? IcContinuo { get; set; }
        public string NuProcessoCliente { get; set; }
        public DateTime? DtInicioExecucao { get; set; }
        public DateTime? DtFimExecucao { get; set; }
        public int? IdArea { get; set; }
        public bool? IcInformacoesIncompletas { get; set; }
        public int? IdSituacaoEquipeTecnica { get; set; }
        public decimal? VlTotalEquipeTecnica { get; set; }
        public decimal? VlTotalTaxaInstitucional { get; set; }
        public decimal? VlCustoProjeto { get; set; }
        public decimal? VlOutrosCustos { get; set; }
        public decimal? VlOverhead { get; set; }
        public decimal? VlTotalCustoProjeto { get; set; }
        public decimal? VlDiferenca { get; set; }
        public bool? IcViaFipeNaoAssinada { get; set; }
        public string NuContratoEdit { get; set; }
        public int? IdContaCorrente { get; set; }
        public bool? chkContratoAbono { get; set; }   //EGS 30.12.2020 Se contrato é de abono, dai nao obriga ter valor


        public virtual Area IdAreaNavigation { get; set; }
        public virtual ContaCorrente IdContaCorrenteNavigation { get; set; }
        public virtual FormaPagamento IdFormaPagamentoNavigation { get; set; }
        public virtual FundamentoContratacao IdFundamentoNavigation { get; set; }
        public virtual IndiceReajuste IdIndiceReajusteNavigation { get; set; }
        public virtual Proposta IdPropostaNavigation { get; set; }
        public virtual Situacao IdSituacaoEquipeTecnicaNavigation { get; set; }
        public virtual Situacao IdSituacaoNavigation { get; set; }
        public virtual Tema IdTemaNavigation { get; set; }
        public virtual TipoApresentacaoRelatorio IdTipoApresentacaoRelatorioNavigation { get; set; }
        public virtual TipoCobranca IdTipoCobrancaNavigation { get; set; }
        public virtual TipoEntregaDocumento IdTipoEntregaDocumentoNavigation { get; set; }
        public virtual ICollection<ContratoAditivo> ContratoAditivo { get; set; }
        public virtual ICollection<ContratoCliente> ContratoCliente { get; set; }
        public virtual ICollection<ContratoComentario> ContratoComentario { get; set; }
        public virtual ICollection<ContratoContatos> ContratoContatos { get; set; }
        public virtual ICollection<ContratoCoordenador> ContratoCoordenador { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiro> ContratoCronogramaFinanceiro { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroTemporaria> ContratoCronogramaFinanceiroTemporaria { get; set; }
        public virtual ICollection<ContratoDoc> ContratoDoc { get; set; }
        public virtual ICollection<ContratoDocPrincipal> ContratoDocPrincipal { get; set; }
        public virtual ICollection<ContratoDocsAcompanhaNf> ContratoDocsAcompanhaNf { get; set; }
        public virtual ICollection<ContratoEntregavel> ContratoEntregavel { get; set; }
        public virtual ICollection<ContratoEntregavelTemporaria> ContratoEntregavelTemporaria { get; set; }
        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
        public virtual ICollection<ContratoHistorico> ContratoHistorico { get; set; }
        public virtual ICollection<ContratoProrrogacao> ContratoProrrogacao { get; set; }
        public virtual ICollection<ContratoReajuste> ContratoReajuste { get; set; }
        public virtual ICollection<Frente> Frente { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
