using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class IndiceReajuste
    {
        public IndiceReajuste()
        {
            Contrato = new HashSet<Contrato>();
            ContratoReajuste = new HashSet<ContratoReajuste>();
            Proposta = new HashSet<Proposta>();
        }

        public short IdIndiceReajuste { get; set; }
        public string DsIndiceReajuste { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
        public virtual ICollection<ContratoReajuste> ContratoReajuste { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
