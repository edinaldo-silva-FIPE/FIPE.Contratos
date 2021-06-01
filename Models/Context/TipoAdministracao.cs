using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoAdministracao
    {
        public TipoAdministracao()
        {
            HistoricoPessoaJuridica = new HashSet<HistoricoPessoaJuridica>();
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdTipoAdministracao { get; set; }
        public string DsTipoAdministracao { get; set; }

        public virtual ICollection<HistoricoPessoaJuridica> HistoricoPessoaJuridica { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
