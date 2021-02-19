using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoDocsAcompanhaNf
    {
        public int IdContratoDocsAcompanhaNf { get; set; }
        public int IdContrato { get; set; }
        public int IdTipoDocsAcompanhaNf { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual TipoDocsAcompanhaNf IdTipoDocsAcompanhaNfNavigation { get; set; }
    }
}
