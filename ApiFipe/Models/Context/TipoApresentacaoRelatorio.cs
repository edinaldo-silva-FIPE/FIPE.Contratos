using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoApresentacaoRelatorio
    {
        public TipoApresentacaoRelatorio()
        {
            Contrato = new HashSet<Contrato>();
        }

        public int IdTipoApresentacaoRelatorio { get; set; }
        public string DsTipoApresentacaoRelatorio { get; set; }

        public virtual ICollection<Contrato> Contrato { get; set; }
    }
}
