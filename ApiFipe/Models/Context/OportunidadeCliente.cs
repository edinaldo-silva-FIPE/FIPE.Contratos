using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class OportunidadeCliente
    {
        public int IdOportunidadeCliente { get; set; }
        public int? IdOportunidade { get; set; }
        public int? IdCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string NmFantasia { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Oportunidade IdOportunidadeNavigation { get; set; }
    }
}
