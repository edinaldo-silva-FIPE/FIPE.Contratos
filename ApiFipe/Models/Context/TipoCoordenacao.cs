using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoCoordenacao
    {
        public TipoCoordenacao()
        {
            ContratoCoordenador = new HashSet<ContratoCoordenador>();
        }

        public int IdTipoCoordenacao { get; set; }
        public string DsTipoCoordenacao { get; set; }

        public virtual ICollection<ContratoCoordenador> ContratoCoordenador { get; set; }
    }
}
