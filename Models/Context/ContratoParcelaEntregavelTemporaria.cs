using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoParcelaEntregavelTemporaria
    {
        public int IdContratoParcelaEntregavel { get; set; }
        public int IdParcela { get; set; }
        public int IdEntregavel { get; set; }

        public virtual ContratoEntregavelTemporaria IdEntregavelNavigation { get; set; }
        public virtual ContratoCronogramaFinanceiroTemporaria IdParcelaNavigation { get; set; }
    }
}
