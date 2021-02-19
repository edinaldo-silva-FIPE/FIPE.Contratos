using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Usuario
    {
        public Usuario()
        {
            ContratoComentario = new HashSet<ContratoComentario>();
            ContratoHistorico = new HashSet<ContratoHistorico>();
            HistoricoPessoaJuridica = new HashSet<HistoricoPessoaJuridica>();
            PerfilUsuario = new HashSet<PerfilUsuario>();
            PessoaFisica = new HashSet<PessoaFisica>();
            PessoaJuridica = new HashSet<PessoaJuridica>();
            Proposta = new HashSet<Proposta>();
            PropostaComentario = new HashSet<PropostaComentario>();
            PropostaHistorico = new HashSet<PropostaHistorico>();
        }

        public int IdUsuario { get; set; }
        public int IdPessoa { get; set; }
        public string DsLogin { get; set; }
        public string CdSenha { get; set; }
        public Guid? NrToken { get; set; }

        public virtual PessoaFisica IdPessoaNavigation { get; set; }
        public virtual ICollection<ContratoComentario> ContratoComentario { get; set; }
        public virtual ICollection<ContratoHistorico> ContratoHistorico { get; set; }
        public virtual ICollection<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual ICollection<PerfilUsuario> PerfilUsuario { get; set; }
        public virtual ICollection<PessoaFisica> PessoaFisica { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
        public virtual ICollection<PropostaComentario> PropostaComentario { get; set; }
        public virtual ICollection<PropostaHistorico> PropostaHistorico { get; set; }
    }
}
