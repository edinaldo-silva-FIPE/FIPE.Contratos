using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoEntidade
    {
        public TipoEntidade()
        {
            Entidade = new HashSet<Entidade>();
        }

        public int IdTipoEntidade { get; set; }
        public string DsTipoEntidade { get; set; }

        public virtual ICollection<Entidade> Entidade { get; set; }
    }
}
