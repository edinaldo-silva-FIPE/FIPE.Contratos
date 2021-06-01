using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Entidade
    {
        public Entidade()
        {
            PessoaJuridica = new HashSet<PessoaJuridica>();
        }

        public int IdEntidade { get; set; }
        public string DsEntidade { get; set; }
        public int? IdTipoEntidade { get; set; }

        public virtual TipoEntidade IdTipoEntidadeNavigation { get; set; }
        public virtual ICollection<PessoaJuridica> PessoaJuridica { get; set; }
    }
}
