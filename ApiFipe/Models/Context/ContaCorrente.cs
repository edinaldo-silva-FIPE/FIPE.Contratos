using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContaCorrente
    {
        public ContaCorrente()
        {
            Contrato = new HashSet<Contrato>();
        }

        public int IdContaCorrente { get; set; }
        public string NmBanco { get; set; }
        public string CdBanco { get; set; }
        public string CdAgencia { get; set; }
        public string NuConta { get; set; }
        public string NuContaEditado { get; set; }
        public bool? IcPadrao { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
