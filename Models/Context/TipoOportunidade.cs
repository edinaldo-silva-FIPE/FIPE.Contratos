using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoOportunidade
    {
        public TipoOportunidade()
        {
            Oportunidade = new HashSet<Oportunidade>();
            Proposta = new HashSet<Proposta>();
        }

        public int IdTipoOportunidade { get; set; }
        public string DsTipoOportunidade { get; set; }
        public string IcModulo { get; set; }

        public virtual ICollection<Oportunidade> Oportunidade { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
