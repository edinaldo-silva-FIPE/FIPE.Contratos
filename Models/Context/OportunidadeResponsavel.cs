using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class OportunidadeResponsavel
    {
        public int IdOportunidadeResponsavel { get; set; }
        public int IdOportunidade { get; set; }
        public int IdPessoaFisica { get; set; }

        public virtual Oportunidade IdOportunidadeNavigation { get; set; }
        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
    }
}
