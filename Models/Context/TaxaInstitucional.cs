using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TaxaInstitucional
    {
        public TaxaInstitucional()
        {
            ContratoEquipeTecnica = new HashSet<ContratoEquipeTecnica>();
        }

        public int IdTaxaInstitucional { get; set; }
        public string DsTaxaInstitucional { get; set; }
        public decimal PcTaxaInstitucional { get; set; }

        public virtual ICollection<ContratoEquipeTecnica> ContratoEquipeTecnica { get; set; }
    }
}
