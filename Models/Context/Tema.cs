using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class Tema
    {
        public Tema()
        {
            Contrato = new HashSet<Contrato>();
            Proposta = new HashSet<Proposta>();
        }

        public int IdTema { get; set; }
        public string DsTema { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
        public virtual ICollection<Proposta> Proposta { get; set; }
    }
}
