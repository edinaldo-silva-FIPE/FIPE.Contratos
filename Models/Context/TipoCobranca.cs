using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoCobranca
    {
        public TipoCobranca()
        {
            Contrato = new HashSet<Contrato>();
        }

        public short IdTipoCobranca { get; set; }
        public string DsTipoCobranca { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
