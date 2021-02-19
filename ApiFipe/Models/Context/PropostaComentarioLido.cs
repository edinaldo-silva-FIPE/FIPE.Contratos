using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class PropostaComentarioLido
    {
        public int IdPropostaComentarioLido { get; set; }
        public int IdPropostaComentario { get; set; }
        public int IdUsuario { get; set; }
    }
}
