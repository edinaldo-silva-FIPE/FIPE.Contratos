using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class FundamentoContratacao
    {
        public FundamentoContratacao()
        {
            Contrato = new HashSet<Contrato>();
            Proposta = new HashSet<Proposta>();
        }

        public short IdFundamento { get; set; }
        public string DsFundamento { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
