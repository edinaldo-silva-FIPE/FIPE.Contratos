using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaFollowUp
    {
        public int IdPropostaFollowUp { get; set; }
        public DateTime? DtFollowUp { get; set; }
        public int? IdProposta { get; set; }
        public string DsFollowUp { get; set; }
        public string DsResultado { get; set; }

        public virtual Proposta IdPropostaNavigation { get; set; }
    }
}
