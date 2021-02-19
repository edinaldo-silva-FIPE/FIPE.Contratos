using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoReajuste
    {
        public TipoReajuste()
        {
            Contrato = new HashSet<Contrato>();
        }

        public short IdTipoReajuste { get; set; }
        public string DsTipoReajuste { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
