using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaCliente
    {
        public int IdPropostaCliente { get; set; }
        public int IdProposta { get; set; }
        public int IdCliente { get; set; }
        public string RazaoSocial { get; set; }
        public string NmFantasia { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Proposta IdPropostaNavigation { get; set; }
    }
}
