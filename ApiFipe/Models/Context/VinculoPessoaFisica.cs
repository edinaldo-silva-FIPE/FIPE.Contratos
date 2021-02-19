using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class VinculoPessoaFisica
    {
        public int IdVinculoPessoa { get; set; }
        public DateTime? DtInicioVinculo { get; set; }
        public DateTime? DtFimVinculo { get; set; }
        public int IdTipoVinculo { get; set; }
        public int IdPessoaFisica { get; set; }

        public virtual PessoaFisica IdPessoaFisicaNavigation { get; set; }
        public virtual TipoVinculo IdTipoVinculoNavigation { get; set; }
    }
}
