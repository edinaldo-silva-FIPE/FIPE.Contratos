using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoComentarioLido
    {
        public int IdContratoComentarioLido { get; set; }
        public int IdContratoComentario { get; set; }
        public int IdUsuario { get; set; }
    }
}
