using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Fornecedor
    {
        public int IdFornecedor { get; set; }
        public int IdPessoa { get; set; }

        public virtual Pessoa IdPessoaNavigation { get; set; }
    }
}
