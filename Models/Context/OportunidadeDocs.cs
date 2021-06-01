using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class OportunidadeDocs
    {
        public int IdOportunidadeDocs { get; set; }
        public int IdOportunidade { get; set; }
        public short IdTipoDocumento { get; set; }
        public string NmDocumento { get; set; }
        public Guid DocFisicoId { get; set; }
        public byte[] DocFisico { get; set; }
        public DateTime DtUpLoad { get; set; }
        public string NmCriador { get; set; }

        public virtual Oportunidade IdOportunidadeNavigation { get; set; }
        public virtual TipoDocumento IdTipoDocumentoNavigation { get; set; }
    }
}
