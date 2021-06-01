using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoFornecedor
    {
        public int IdContratoFornecedor { get; set; }
        public int IdContrato { get; set; }
        public int IdFornecedor { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Fornecedor IdFornecedorNavigation { get; set; }
    }
}
