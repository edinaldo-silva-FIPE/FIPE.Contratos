using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Pais
    {
        public Pais()
        {
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdPais { get; set; }
        public string Iso { get; set; }
        public string Iso3 { get; set; }
        public string Nome { get; set; }

        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
