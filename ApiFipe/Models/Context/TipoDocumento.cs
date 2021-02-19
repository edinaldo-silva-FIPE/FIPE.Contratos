using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class TipoDocumento
    {
        public TipoDocumento()
        {
            ContratoDoc = new HashSet<ContratoDoc>();
            ContratoDocPrincipal = new HashSet<ContratoDocPrincipal>();
            OportunidadeDocs = new HashSet<OportunidadeDocs>();
            PropostaDocs = new HashSet<PropostaDocs>();
        }

        public short IdTipoDoc { get; set; }
        public string DsTipoDoc { get; set; }
        public int IdPrincipais { get; set; }
        public int? IdEntidade { get; set; }
        public bool? IcDocContratual { get; set; }

        public virtual ICollection<ContratoDoc> ContratoDoc { get; set; }
        public virtual ICollection<ContratoDocPrincipal> ContratoDocPrincipal { get; set; }
        public virtual ICollection<OportunidadeDocs> OportunidadeDocs { get; set; }
        public virtual ICollection<PropostaDocs> PropostaDocs { get; set; }
    }
}
