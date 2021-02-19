using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaComentario
    {
        public int IdPropostaComentario { get; set; }
        public string DsComentario { get; set; }
        public DateTime DtComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdProposta { get; set; }

        public virtual Proposta IdPropostaNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
