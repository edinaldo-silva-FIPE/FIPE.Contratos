using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoDoc
    {
        public int IdContratoDoc { get; set; }
        public int IdContrato { get; set; }
        public short IdTipoDoc { get; set; }
        public string DsDoc { get; set; }
        public Guid DocFisicoId { get; set; }
        public byte[] DocFisico { get; set; }
        public DateTime DtUpload { get; set; }
        public string NmCriador { get; set; }
        public DateTime DtDoc { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual TipoDocumento IdTipoDocNavigation { get; set; }
    }
}
