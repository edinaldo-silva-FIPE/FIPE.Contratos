using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoFornecedorDoc
    {
        public int IdContratoFornecedorDoc { get; set; }
        public int IdContratoFornecedor { get; set; }
        public short IdTipoDoc { get; set; }
        public string DsDoc { get; set; }
        public Guid DocFisicoId { get; set; }
        public byte[] DocFisico { get; set; }

        public virtual TipoDocumento IdTipoDocNavigation { get; set; }
    }
}
