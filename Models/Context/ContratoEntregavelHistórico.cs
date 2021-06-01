using System;
using System.Collections.Generic;

namespace ApiFipe.Models.Context
{
    public partial class ContratoEntregavelHistórico
    {
        public long IdContratoEntregavelHistorico { get; set; }
        public int IdContratoAditivo { get; set; }
        public int IdContratoCliente { get; set; }
        public int? NuEntregavel { get; set; }
        public int IdSituacao { get; set; }
        public int IdFrente { get; set; }
        public string DsProduto { get; set; }
        public DateTime? DtProduto { get; set; }
        public int VlOrdem { get; set; }

        public virtual ContratoAditivo IdContratoAditivoNavigation { get; set; }
        public virtual ContratoCliente IdContratoClienteNavigation { get; set; }
    }
}
