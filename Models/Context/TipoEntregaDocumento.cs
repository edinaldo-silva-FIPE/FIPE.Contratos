using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoEntregaDocumento
    {
        public TipoEntregaDocumento()
        {
            Contrato = new HashSet<Contrato>();
        }

        public short IdTipoEntregaDocumento { get; set; }
        public string DsTipoEntregaDocumento { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
