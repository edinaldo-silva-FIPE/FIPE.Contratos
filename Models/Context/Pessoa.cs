using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Pessoa
    {
        public Pessoa()
        {
            Cliente = new HashSet<Cliente>();
            Fornecedor = new HashSet<Fornecedor>();
        }

        public int IdPessoa { get; set; }
        public int? IdPessoaFisica { get; set; }
        public int? IdPessoaJuridica { get; set; }

        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
        public virtual PessoaJuridica IdPessoaJuridicaNavigation { get; set; }
        public virtual ICollection<Cliente> Cliente { get; set; }
        public virtual ICollection<Fornecedor> Fornecedor { get; set; }
    }
}
