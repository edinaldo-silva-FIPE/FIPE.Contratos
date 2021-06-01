using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class FrenteCoordenador
    {
        public int IdFrenteCoordenador { get; set; }
        public int IdPessoaFisica { get; set; }
        public int IdFrente { get; set; }

        public virtual Frente IdFrenteNavigation { get; set; }
        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
    }
}
