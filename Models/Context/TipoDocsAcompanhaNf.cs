using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoDocsAcompanhaNf
    {
        public TipoDocsAcompanhaNf()
        {
            ContratoDocsAcompanhaNf = new HashSet<ContratoDocsAcompanhaNf>();
        }

        public int IdTipoDocsAcompanhaNf { get; set; }
        public string DsTipoDocsAcompanhaNf { get; set; }
        public bool? IcPadrao { get; set; }

        public virtual ICollection<ContratoDocsAcompanhaNf> ContratoDocsAcompanhaNf { get; set; }
    }
}
