using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoComentario
    {
        public int IdContratoComentario { get; set; }
        public string DsComentario { get; set; }
        public DateTime DtComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdContrato { get; set; }

        public virtual Contrato IdContratoNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
