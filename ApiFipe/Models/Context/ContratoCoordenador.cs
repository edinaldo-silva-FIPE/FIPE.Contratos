using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoCoordenador
    {
        public int IdContratoCoordenador { get; set; }
        public int IdContrato { get; set; }
        public int IdPessoa { get; set; }
        public int? IdTipoCoordenacao { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual PessoaFisica IdPessoaNavigation { get; set; }
        public virtual TipoCoordenacao IdTipoCoordenacaoNavigation { get; set; }
    }
}
