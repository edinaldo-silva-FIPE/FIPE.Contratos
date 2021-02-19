using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoParcelaEntregavel
    {
        public int IdContratoParcelaEntregavel { get; set; }
        public int IdParcela { get; set; }
        public int IdEntregavel { get; set; }

        public virtual ContratoEntregavel IdEntregavelNavigation { get; set; }
        public virtual ContratoCronogramaFinanceiro IdParcelaNavigation { get; set; }
    }
}
