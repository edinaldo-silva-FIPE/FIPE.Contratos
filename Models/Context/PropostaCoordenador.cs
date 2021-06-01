using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaCoordenador
    {
        public int IdPropostaCoordenador { get; set; }
        public int IdProposta { get; set; }
        public int? IdPessoa { get; set; }
        public bool? IcAprovado { get; set; }
        public bool? IcPropostaAprovada { get; set; }
        public bool? IcAnaliseLiberada { get; set; }
        public Guid? GuidPropostaCoordenador { get; set; }

        public virtual PessoaFisica IdPessoaNavigation { get; set; }
        public virtual Proposta IdPropostaNavigation { get; set; }
    }
}
