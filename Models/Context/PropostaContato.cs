using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaContato
    {
        public int IdPropostaContato { get; set; }
        public int? IdProposta { get; set; }
        public string NmContato { get; set; }
        public string CdEmail { get; set; }
        public string NuTelefone { get; set; }
        public string NuCelular { get; set; }
        public string NmDepartamento { get; set; }
        public int? IdTipoContato { get; set; }

        public virtual TipoContato IdTipoContatoNavigation { get; set; }
    }
}
