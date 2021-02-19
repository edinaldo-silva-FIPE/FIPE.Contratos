using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class UnidadeDeTempo
    {
        public UnidadeDeTempo()
        {
            PropostaIdUnidadeTempoJuridicoNavigation = new HashSet<Proposta>();
            PropostaIdUnidadeTempoNavigation = new HashSet<Proposta>();
        }

        public short IdUnidadeTempo { get; set; }
        public string DsUnidadeTempo { get; set; }

        public virtual ICollection<Proposta> PropostaIdUnidadeTempoJuridicoNavigation { get; set; }
        public virtual ICollection<Proposta> PropostaIdUnidadeTempoNavigation { get; set; }
    }
}
