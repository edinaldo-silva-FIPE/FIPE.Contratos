using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class GrauOportunidade
    {
        public GrauOportunidade()
        {
            Oportunidade = new HashSet<Oportunidade>();
        }

        public int IdGrauOportunidade { get; set; }
        public string DsGrauOportunidade { get; set; }

        public virtual ICollection<Oportunidade> Oportunidade { get; set; }
    }
}
