using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoContratacaoEquipeTecnica
    {
        public TipoContratacaoEquipeTecnica()
        {
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
        }

        public int IdTipoContratacao { get; set; }
        public string DsTipoContratacao { get; set; }

        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
    }
}
