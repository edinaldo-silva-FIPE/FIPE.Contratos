using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class OportunidadeContato
    {
        public int IdOportunidadeContato { get; set; }
        public int? IdOportunidade { get; set; }
        public string NmContato { get; set; }
        public string CdEmail { get; set; }
        public string NuTelefone { get; set; }
        public string NuCelular { get; set; }
        public string NmDepartamento { get; set; }
        public int? IdTipoContato { get; set; }

        public virtual TipoContato IdTipoContatoNavigation { get; set; }
    }
}
