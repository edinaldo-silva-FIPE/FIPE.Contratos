using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Area
    {
        public Area()
        {
            Contrato = new HashSet<Contrato>();
        }

        public int IdArea { get; set; }
        public string DsArea { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
