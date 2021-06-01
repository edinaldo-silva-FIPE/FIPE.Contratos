using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoAditivo
    {
        public TipoAditivo()
        {
            ContratoAditivo = new HashSet<ContratoAditivo>();
            Proposta = new HashSet<Proposta>();
        }

        public short IdTipoAditivo { get; set; }
        public string DsTipoAditivo { get; set; }

        public virtual ICollection<ContratoAditivo> ContratoAditivo { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
