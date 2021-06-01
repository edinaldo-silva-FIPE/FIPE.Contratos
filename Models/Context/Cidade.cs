using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Cidade
    {
        public Cidade()
        {
            HistoricoPessoaJuridica = new HashSet<HistoricoPessoaJuridica>();
            PessoaFisica = new HashSet<PessoaFisica>();
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdCidade { get; set; }
        public string NmCidade { get; set; }
        public int? CdIbge { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Uf { get; set; }
        public int? CdUf { get; set; }

        public virtual ICollection<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual ICollection<PessoaFisica> PessoaFisica { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
