using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class FormaPagamento
    {
        public FormaPagamento()
        {
            Contrato = new HashSet<Contrato>();
        }

        public int IdFormaPagamento { get; set; }
        public string DsFormaPagamento { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
