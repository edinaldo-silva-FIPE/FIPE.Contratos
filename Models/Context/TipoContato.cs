using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoContato
    {
        public TipoContato()
        {
            ContratoContatos = new HashSet<ContratoContatos>();
            OportunidadeContato = new HashSet<OportunidadeContato>();
            PropostaContato = new HashSet<PropostaContato>();
        }

        public int IdTipoContato { get; set; }
        public string DsTipoContato { get; set; }

        public virtual ICollection<ContratoContatos> ContratoContatos { get; set; }
        public virtual ICollection<OportunidadeContato> OportunidadeContato { get; set; }
        public virtual ICollection<PropostaContato> PropostaContato { get; set; }
    }
}
