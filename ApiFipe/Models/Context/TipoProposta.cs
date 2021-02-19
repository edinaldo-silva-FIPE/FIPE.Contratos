using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoProposta
    {
        public TipoProposta()
        {
            Proposta = new HashSet<Proposta>();
        }

        public int IdTipoProposta { get; set; }
        public string DsTipoProposta { get; set; }

        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
