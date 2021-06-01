using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class FormacaoProfissional
    {
        public FormacaoProfissional()
        {
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
        }

        public int IdFormacaoProfissional { get; set; }
        public string DsFormacaoProfissional { get; set; }

        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
    }
}
