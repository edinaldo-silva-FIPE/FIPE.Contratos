using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Situacao
    {
        public Situacao()
        {
            ContratoAditivo = new HashSet<ContratoAditivo>();
            ContratoCronogramaFinanceiro = new HashSet<ContratoCronogramaFinanceiro>();
            ContratoCronogramaFinanceiroTemporaria = new HashSet<ContratoCronogramaFinanceiroTemporaria>();
            ContratoEntregavel = new HashSet<ContratoEntregavel>();
            ContratoEntregavelTemporaria = new HashSet<ContratoEntregavelTemporaria>();
            ContratoHistorico = new HashSet<ContratoHistorico>();
            ContratoIdSituacaoEquipeTecnicaNavigation = new HashSet<Contrato>();
            ContratoIdSituacaoNavigation = new HashSet<Contrato>();
            ContratoProrrogacao = new HashSet<ContratoProrrogacao>();
            ContratoReajuste = new HashSet<ContratoReajuste>();
            Oportunidade = new HashSet<Oportunidade>();
            Proposta = new HashSet<Proposta>();
            PropostaHistorico = new HashSet<PropostaHistorico>();
        }

        public int IdSituacao { get; set; }
        public string DsSituacao { get; set; }
        public string IcEntidade { get; set; }
        public string DsArea { get; set; }
        public string DsSubArea { get; set; }
        public bool? IcEntregue { get; set; }
        public bool? IcNfemitida { get; set; }
        public bool? IcFormatacao { get; set; }
        public short? NuOrdem { get; set; }

        public virtual ICollection<ContratoAditivo> ContratoAditivo { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiro> ContratoCronogramaFinanceiro { get; set; }
        public virtual ICollection<ContratoCronogramaFinanceiroTemporaria> ContratoCronogramaFinanceiroTemporaria { get; set; }
        public virtual ICollection<ContratoEntregavel> ContratoEntregavel { get; set; }
        public virtual ICollection<ContratoEntregavelTemporaria> ContratoEntregavelTemporaria { get; set; }
        public virtual ICollection<ContratoHistorico> ContratoHistorico { get; set; }
        public virtual ICollection<Contrato> ContratoIdSituacaoEquipeTecnicaNavigation { get; set; }
        public virtual ICollection<Contrato> ContratoIdSituacaoNavigation { get; set; }
        public virtual ICollection<ContratoProrrogacao> ContratoProrrogacao { get; set; }
        public virtual ICollection<ContratoReajuste> ContratoReajuste { get; set; }
        public virtual ICollection<Oportunidade> Oportunidade { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
        public virtual ICollection<PropostaHistorico> PropostaHistorico { get; set; }
    }
}
