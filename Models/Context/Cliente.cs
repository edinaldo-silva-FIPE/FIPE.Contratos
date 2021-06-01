using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Cliente
    {
        public Cliente()
        {
            ContratoCliente = new HashSet<ContratoCliente>();
            OportunidadeCliente = new HashSet<OportunidadeCliente>();
            PropostaCliente = new HashSet<PropostaCliente>();
        }

        public int IdCliente { get; set; }
        public int IdPessoa { get; set; }

        public virtual Pessoa IdPessoaNavigation { get; set; }
        public virtual ICollection<ContratoCliente> ContratoCliente { get; set; }
        public virtual ICollection<OportunidadeCliente> OportunidadeCliente { get; set; }
        public virtual ICollection<PropostaCliente> PropostaCliente { get; set; }
    }
}
