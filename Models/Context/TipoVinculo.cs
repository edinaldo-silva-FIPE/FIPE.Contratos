using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoVinculo
    {
        public TipoVinculo()
        {
            HistoricoPessoaFisica = new HashSet<HistoricoPessoaFisica>();
            PessoaFisica = new HashSet<PessoaFisica>();
            VinculoPessoaFisica = new HashSet<VinculoPessoaFisica>();
        }

        public int IdTipoVinculo { get; set; }
        public string DsTipoVinculo { get; set; }

        public virtual ICollection<HistoricoPessoaFisica> HistoricoPessoaFisica { get; set; }
        public virtual ICollection<PessoaFisica> PessoaFisica { get; set; }
        public virtual ICollection<VinculoPessoaFisica> VinculoPessoaFisica { get; set; }
    }
}
